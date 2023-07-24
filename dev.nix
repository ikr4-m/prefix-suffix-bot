{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
    name = "indev_env";
    packages = [
		pkgs.dotnet-sdk_7
	];
}