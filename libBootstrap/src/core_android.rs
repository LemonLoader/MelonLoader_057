use jni::{
    objects::{JClass, JString},
    sys::{jboolean, jint, JNI_VERSION_1_6},
    JNIEnv, JavaVM, NativeMethod,
};
use std::{ os::raw::c_void, panic::catch_unwind };

const INVALID_JNI_VERSION: jint = 0;

#[allow(non_snake_case)]
#[no_mangle]
pub extern "system" fn JNI_OnLoad(vm: JavaVM, _: *mut c_void) -> jint {
    info!("Grabbing JNIEnv...");

    let mut env: JNIEnv = vm.get_env().expect("Cannot get reference to the JNIEnv");
    vm.attach_current_thread()
        .expect("Unable to attach current thread to the JVM");

    catch_unwind(|| JNI_VERSION_1_6).unwrap_or(INVALID_JNI_VERSION)
}