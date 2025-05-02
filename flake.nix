{
  description = "MyLox interpreter (Crafting Interpreters, C#)";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-24.11";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };
        dotnet-sdk = pkgs.dotnet-sdk_8;
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = [ dotnet-sdk ];
          DOTNET_ROOT = dotnet-sdk;
          DOTNET_CLI_TELEMETRY_OPTOUT = "1";
        };
      });
}
