# TODO: Nix path pointing is sucks, but I like it.
# 1. Make a list or object to pointing binary folder (maybe we need some new
# derivation to help that?)
# 2. Make a list just for import the libs
# 3. Loop #1 and #2 to mkShell.packages

{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell rec {
    name = "indev_env";
    packages = [
        pkgs.dotnet-sdk_7
    ];

    # Hotfix: Pointing Dotnet Root
    dotnetRoot = builtins.toString pkgs.dotnet-sdk_7;
    shellHook = ''
        echo "[HOOK] Restore all dotnet package."
        dotnet restore
        echo "[HOOK] Import environment variable."
        set -o allexport && source ./.env && set +o allexport
        echo "[HOOK] Set Dotnet root"
        export DOTNET_ROOT=${dotnetRoot}
        echo "[HOOK] Done!"
    '';
}
