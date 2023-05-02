// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
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
            _sandBox.Dispose();
        }

        [Test]
        public void AssertEqualityInLowTrustSandBox()
        {
            _sandBox.Run(() => Assert.That(1, Is.EqualTo(1)));
        }

        [Test]
        public void AssertEqualityWithToleranceInLowTrustSandBox()
        {
            _sandBox.Run(() => Assert.That(10.5, Is.EqualTo(10.5)));
        }

        [Test]
        public void AssertThrowsInLowTrustSandBox()
        {
            _sandBox.Run(() => Assert.Throws<SecurityException>(() => new SecurityPermission(SecurityPermissionFlag.Infrastructure).Demand()));
        }
    }

    /// <summary>
    /// A facade for an <see cref="AppDomain"/> with partial trust privileges.
    /// </summary>
    public class TestSandBox : IDisposable
    {
        private readonly AppDomain _appDomain;

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
        public TestSandBox(PermissionSet? permissions, params Assembly[] fullTrustAssemblies)
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
                "TestSandBox" + DateTime.Now.Ticks, null!, setup,
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
            AppDomain.Unload(_appDomain);
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

        public void Run(Action action)
        {
            Run(action.Method);
        }
        public object? Run(MethodInfo method, params object[] parameters)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (_appDomain == null) throw new ObjectDisposedException(null);

            var methodRunnerType = typeof(MethodRunner);
            var methodRunnerProxy = (MethodRunner?)_appDomain.CreateInstanceAndUnwrap(
                methodRunnerType.Assembly.FullName!, methodRunnerType.FullName!);
            Assert.That(methodRunnerProxy, Is.Not.Null);

            try
            {
                return methodRunnerProxy.Run(method, parameters);
            }
            catch (Exception e)
            {
                throw e is TargetInvocationException
                    ? e.InnerException!
                    : e;
            }
        }

#endregion

#region Private methods

        private static StrongName GetStrongName(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();

            byte[]? publicKey = assembly.GetName().GetPublicKey();
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
            public object? Run(MethodInfo method, params object[] parameters)
            {
                var instance = method.IsStatic
                    ? null
                    : Activator.CreateInstance(method.ReflectedType!);
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
