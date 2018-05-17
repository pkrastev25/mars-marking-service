#!/usr/bin/env bash
# Used to generate the proto server and proto client for the ProjectService.proto file

# Change the path according to your system
protoc=C:/Users/User/.nuget/packages/grpc.tools/1.12.0/tools/windows_x86/protoc.exe
grpc_csharp_plugin=C:/Users/User/.nuget/packages/grpc.tools/1.12.0/tools/windows_x86/grpc_csharp_plugin.exe

${protoc} -I ./ResourceTypes/Project/Proto/ --csharp_out=./ResourceTypes/Project/Proto/ ./ResourceTypes/Project/Proto/ProjectService.proto --grpc_out=./ResourceTypes/Project/Proto/ --plugin=protoc-gen-grpc=${grpc_csharp_plugin}