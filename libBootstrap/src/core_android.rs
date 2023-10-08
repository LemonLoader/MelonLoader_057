use jni::{
    sys::{ jint, JNI_VERSION_1_6},
    JNIEnv, JavaVM,
};
use std::{ os::raw::c_void, panic::catch_unwind };
use crate::{log, melonenv::paths, internal_failure};

const INVALID_JNI_VERSION: jint = 0;

#[allow(non_snake_case)]
#[no_mangle]
pub extern "system" fn JNI_OnLoad(vm: JavaVM, _: *mut c_void) -> jint {
    let mut env: JNIEnv = vm.get_env().expect("Cannot get reference to the JNIEnv");
    vm.attach_current_thread()
        .expect("Unable to attach current thread to the JVM");

    paths::cache_data_dir(&mut env);
    crate::logging::logger::init().expect("Failed to initialize logger!");

    log!("JNI initialized!");

    catch_unwind(|| JNI_VERSION_1_6).unwrap_or(INVALID_JNI_VERSION)
}