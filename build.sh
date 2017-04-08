#!/bin/bash
###############################################################
# This is the Cake bootstrapper script that is responsible for
# downloading Cake and all specified tools from NuGet.
###############################################################

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NUGET_EXE=$TOOLS_DIR/nuget.exe
CAKE_EXE=$TOOLS_DIR/Cake/Cake.exe
CAKE_CORE_VERSION=0.19.3

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=false
SHOW_VERSION=false

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -d|--dryrun) DRYRUN=true ;;
        --version) SHOW_VERSION=true ;;
    esac
    shift
done

if which mono ; then

    # Download NuGet if it does not exist.
    if [ ! -f $NUGET_EXE ]; then
        echo "Downloading NuGet..."
        curl -Lsfo $NUGET_EXE https://www.nuget.org/nuget.exe
        if [ $? -ne 0 ]; then
            echo "An error occured while downloading nuget.exe."
            exit 1
        fi
    fi

    # Restore tools from NuGet.
    pushd $TOOLS_DIR >/dev/null
    mono $NUGET_EXE install -ExcludeVersion
    popd >/dev/null

    # Make sure that Cake has been installed.
    if [ ! -f $CAKE_EXE ]; then
        echo "Could not find Cake.exe."
        exit 1
    fi

    # Start Cake
    if $SHOW_VERSION; then
        mono $CAKE_EXE -version
    elif $DRYRUN; then
        mono $CAKE_EXE $SCRIPT -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET -dryrun
    else
        mono $CAKE_EXE $SCRIPT -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET
    fi

    exit $?

else

    # Make sure the tools folder exist.
    if [ ! -d "$TOOLS_DIR" ]; then
        mkdir "$TOOLS_DIR"
    fi

    ###########################################################################
    # INSTALL .NET CORE CLI
    ###########################################################################

    if ! [ -x "$(command -v dotnet)" ]; then
        if [ ! -f "$SCRIPT_DIR/.dotnet/dotnet" ]; then
            echo "Installing .NET CLI..."
            if [ ! -d "$SCRIPT_DIR/.dotnet" ]; then
                mkdir "$SCRIPT_DIR/.dotnet"
            fi
            # Install .NET CLI dependencies
            if [ -x "$(command -v apt-get)" ]; then
                sudo apt-get update \
                    && sudo apt-get install -y --no-install-recommends \
                        libc6 \
                        libcurl3 \
                        libgcc1 \
                        libgssapi-krb5-2 \
                        libicu52 \
                        liblttng-ust0 \
                        libssl1.0.0 \
                        libstdc++6 \
                        libunwind8 \
                        libuuid1 \
                        zlib1g \
                    && sudo rm -rf /var/lib/apt/lists/*
            fi
            curl -Lsfo "$SCRIPT_DIR/.dotnet/dotnet-install.sh" https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview2/scripts/obtain/dotnet-install.sh
            sudo bash "$SCRIPT_DIR/.dotnet/dotnet-install.sh" --version 1.0.0-preview2-003121 --install-dir .dotnet --no-path
            export PATH="$SCRIPT_DIR/.dotnet":$PATH
        fi
    fi
    export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
    export DOTNET_CLI_TELEMETRY_OPTOUT=1
    dotnet --info

    ###########################################################################
    # INSTALL CAKE
    ###########################################################################

    if [ ! -f "$CAKE_EXE" ]; then
        if [ ! -f "$TOOLS_DIR/project.json" ]; then
            echo '{"dependencies":{"Cake.CoreCLR": "'$CAKE_CORE_VERSION'"},"frameworks":{"netstandard1.6":{}}}' > "$TOOLS_DIR/project.json"
        fi
        exec dotnet restore "$TOOLS_DIR" --packages "$TOOLS_DIR"
        if [ $? -ne 0 ]; then
            echo "An error occured while installing Cake."
            exit 1
        fi
    fi

    # Make sure that Cake has been installed.
    if [ ! -f "$CAKE_EXE" ]; then
        echo "Could not find Cake.exe at '$CAKE_EXE'."
        exit 1
    fi

    ###########################################################################
    # RUN BUILD SCRIPT
    ###########################################################################

    # Start Cake
    dotnet "$CAKE_EXE" build.cake --verbosity=$VERBOSITY --configuration=$CONFIGURATION --target=$TARGET $DRYRUN "${SCRIPT_ARGUMENTS[@]}"

fi