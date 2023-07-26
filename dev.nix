{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
    name = "indev_env";
    packages = [
        pkgs.dotnet-sdk_7
    ];
    shellHook = ''
        echo "[HOOK] Restore all dotnet package."
        dotnet restore
        echo "[HOOK] Import environment variable."
        set -o allexport && source ./.env && set +o allexport
        echo "[HOOK] Done!"
    '';
}