@echo off
cargo ndk -t arm64-v8a -o ./jniLibs build --release
copy "%~dp0jniLibs\arm64-v8a\libmain.so" "C:/Users/trevo/Desktop/android_2d_bw/lib/arm64-v8a"
IF "%1" NEQ "auto" (
    pause
)