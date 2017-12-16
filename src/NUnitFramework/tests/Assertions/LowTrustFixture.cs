// ***********************************************************************
// Copyright (c) 2004 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if NET20 || NET35 || NET40 || NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    [Platform(Exclude = "Mono,MonoTouch", Reason = "Mono does not implement Code Access Security")]
    public class LowTrustFixture
    {
        private TestSandBox _sandBox;

        [OneTimeSetUp]
        public void CreateSandBox()
        {
            _sandBox = new TestSandBox();
        }

        [OneTimeTearDown]
        public void DisposeSandBox()
        {
            if (_sandBox != null)
            {
                _sandBox.Dispose();
                _sandBox = null;
            }
        }

        [Test]
        public void AssertEqualityInLowTrustSandBox()
        {
            _sandBox.Run(() =>
            {
                Assert.That(1, Is.EqualTo(1));
            });
        }

        [Test]
        public void AssertEqualityWithToleranceInLowTrustSandBox()
        {
            _sandBox.Run(() =>
            {
                Assert.That(10.5, Is.EqualTo(10.5));
            });
        }

        [Test]
        public void AssertThrowsInLowTrustSandBox()
        {
            _sandBox.Run(() =>
            {
                Assert.Throws<SecurityException>(() => new SecurityPermission(SecurityPermissionFlag.Infrastructure).Demand());
            });
        }
    }

    /// <summary>
    /// A facade for an <see cref="AppDomain"/> with partial trust privileges.
    /// </summary>
    public class TestSandBox : IDisposable
    {
        private AppDomain _appDomain;

#region Constructor(s)

        /// <summary>
        /// Creates a low trust <see cref="TestSandBox"/> instance.
        /// </summary>
        /// <param name="fullTrustAssemblies">Strong named assemblies that will have full trust in the sandbox.</param>
        public TestSandBox(params Assembly[] fullTrustAssemblies)
            : this(null, fullTrustAssemblies)
        { }

        /// <summary>
        /// Creates a partial trust <see cref="TestSandBox"/> instance with a given set of permissions.
        /// </summary>
        /// <param name="permissions">Optional <see cref="TestSandBox"/> permission set. By default a minimal trust
        /// permission set is used.</param>
        /// <param name="fullTrustAssemblies">Strong named assemblies that will have full trust in the sandbox.</param>
        public TestSandBox(PermissionSet permissions, params Assembly[] fullTrustAssemblies)
        {
            var setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };

            var strongNames = new HashSet<StrongName>();

            // Grant full trust to NUnit.Framework assembly to enable use of NUnit assertions in sandboxed test code.
            strongNames.Add(GetStrongName(typeof(TestAttribute).Assembly));
            if (fullTrustAssemblies != null)
            {
                foreach (var assembly in fullTrustAssemblies)
                {
                    strongNames.Add(GetStrongName(assembly));
                }
            }

            _appDomain = AppDomain.CreateDomain(
                "TestSandBox" + DateTime.Now.Ticks, null, setup,
                permissions ?? GetLowTrustPermissionSet(),
                strongNames.ToArray());
        }

#endregion

#region Finalizer and Dispose methods

        /// <summary>
        /// The <see cref="TestSandBox"/> finalizer.
        /// </summary>
        ~TestSandBox()
        {
            Dispose(false);
        }

        /// <summary>
        /// Unloads the <see cref="AppDomain"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Unloads the <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether this method is called from <see cref="Dispose()"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_appDomain != null)
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }

#endregion

#region PermissionSet factory methods
        public static PermissionSet GetLowTrustPermissionSet()
        {
            var permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new SecurityPermission(
                SecurityPermissionFlag.Execution |                  // Required to execute test code
                SecurityPermissionFlag.SerializationFormatter));    // Required to support cross-appdomain test result formatting by NUnit TestContext
            permissions.AddPermission(new ReflectionPermission(
                ReflectionPermissionFlag.MemberAccess));            // Required to instantiate classes that contain test code and to get cross-appdomain communication to work.
            return permissions;
        }

#endregion

#region Run methods

        public T Run<T>(Func<T> func)
        {
            return (T)Run(func.Method);
        }

        public void Run(Action action)
        {
            Run(action.Method);
        }
        public object Run(MethodInfo method, params object[] parameters)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (_appDomain == null) throw new ObjectDisposedException(null);

            var methodRunnerType = typeof(MethodRunner);
            var methodRunnerProxy = (MethodRunner)_appDomain.CreateInstanceAndUnwrap(
                methodRunnerType.Assembly.FullName, methodRunnerType.FullName);

            try
            {
                return methodRunnerProxy.Run(method, parameters);
            }
            catch (Exception e)
            {
                throw e is TargetInvocationException
                    ? e.InnerException
                    : e;
            }
        }

#endregion

#region Private methods

        private static StrongName GetStrongName(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();

            byte[] publicKey = assembly.GetName().GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
            {
                throw new InvalidOperationException("Assembly is not strongly named");
            }

            return new StrongName(new StrongNamePublicKeyBlob(publicKey), assemblyName.Name, assemblyName.Version);
        }

#endregion

#region Inner classes

        [Serializable]
        internal class MethodRunner : MarshalByRefObject
        {
            public object Run(MethodInfo method, params object[] parameters)
            {
                var instance = method.IsStatic
                    ? null
                    : Activator.CreateInstance(method.ReflectedType);
                try
                {
                    return method.Invoke(instance, parameters);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException == null) throw;
                    throw e.InnerException;
                }
            }
        }

#endregion
    }
}
#endif
