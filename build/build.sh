#!/usr/bin/env bash

set -xe

cd ../

MOD_NAME="FastReset"
VERSION="$(git describe --abbrev=0 | tr -d  "v")"

BP_NAME="$MOD_NAME-$VERSION-BepInEx"
BP_DIR="build/$BP_NAME"


dotnet build -c Release-BepInEx

mkdir -p "$BP_DIR"/plugins

# BepInEx
cp bin/release-bepinex/net472/${MOD_NAME}.dll \
    "$BP_DIR/plugins/"
cp build/README-BepInEx.txt "$BP_DIR/README.txt"

# Zip everything
pushd "$BP_DIR"
zip -r ../"$BP_NAME.zip" .
popd

# Remove directories
rm -rf "$BP_DIR"
