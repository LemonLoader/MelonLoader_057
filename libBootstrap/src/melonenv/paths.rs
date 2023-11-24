use std::path::{Path, PathBuf};

use lazy_static::lazy_static;
use unity_rs::runtime::RuntimeType;

use crate::{
    constants::W, runtime, utils::runtime::{self, NetstandardVersion},
    errors::DynErr, internal_failure,
};

use super::args::ARGS;

#[cfg(target_os = "android")]
use jni::{
    objects::{JObject, JString}, JNIEnv
};

lazy_static! {
    pub static ref BASE_DIR: W<PathBuf> = {
        let args: Vec<String> = std::env::args().collect();
        let mut base_dir = current_dir()
            .unwrap_or_else(|e| internal_failure!("Failed to get base dir: {e}"));
        for arg in args.iter() {
            if arg.starts_with("--melonloader.basedir") {
                let a: Vec<&str> = arg.split("=").collect();
                base_dir = PathBuf::from(a[1]);
            }
        }

        W(base_dir)
    };
    pub static ref GAME_DIR: W<PathBuf> = {
        let args: Vec<String> = std::env::args().collect();
        let mut base_dir = current_dir()
            .unwrap_or_else(|e| internal_failure!("Failed to get game dir: {e}"));
        for arg in args.iter() {
            if arg.starts_with("--melonloader.basedir") {
                let a: Vec<&str> = arg.split("=").collect();
                base_dir = PathBuf::from(a[1]);
            }
        }

        W(base_dir)
    };
    pub static ref MELONLOADER_FOLDER: W<PathBuf> = W(BASE_DIR.0.join("MelonLoader"));
    pub static ref DEPENDENCIES_FOLDER: W<PathBuf> = W(MELONLOADER_FOLDER.0.join("Dependencies"));
    pub static ref SUPPORT_MODULES_FOLDER: W<PathBuf> =
        W(DEPENDENCIES_FOLDER.0.join("SupportModules"));
    pub static ref PRELOAD_DLL: W<PathBuf> = W(SUPPORT_MODULES_FOLDER.0.join("Preload.dll"));
}

static mut DATA_DIR: Option<String> = None;

pub fn runtime_dir() -> Result<PathBuf, DynErr> {
    let runtime = runtime!()?;

    let mut path = MELONLOADER_FOLDER.clone();

    //let version = runtime::get_netstandard_version()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => path.push("net35"),
        RuntimeType::Il2Cpp(_) => path.push("net8"),
    }

    Ok(path.to_path_buf())
}

pub fn get_managed_dir() -> Result<PathBuf, DynErr> {
    let file_path = std::env::current_exe()?;

    let file_name = file_path
        .file_stem()
        .ok_or_else(|| "Failed to get File Stem!")?
        .to_str()
        .ok_or_else(|| "Failed to get File Stem!")?;

    let base_folder = file_path.parent().ok_or_else(|| "Data Path not found!")?;

    let managed_path = base_folder
        .join(format!("{}_Data", file_name))
        .join("Managed");

    match managed_path.exists() {
        true => Ok(managed_path),
        false => {
            let managed_path = base_folder.join("MelonLoader").join("Managed");

            match managed_path.exists() {
                true => Ok(managed_path),
                false => Err("Failed to find the managed directory!")?,
            }
        }
    }
}

#[cfg(not(target_os = "android"))]
pub fn current_dir() -> std::io::Result<PathBuf> {
    let mut path = std::env::current_dir()?;
    path.pop();
    Ok(path)
}

#[cfg(target_os = "android")]
pub fn current_dir() -> Result<PathBuf, DynErr> {
    unsafe {
        match &DATA_DIR {
            Some(cached_dir) => Ok(PathBuf::from(cached_dir)),
            None => internal_failure!("Failed to get cached data path!"),
        }
    }
}

#[cfg(target_os = "android")]
pub fn cache_data_dir(env: &mut JNIEnv) {
    use jni::objects::JValueGen;

    use crate::log;

    let unity_class_name = "com/unity3d/player/UnityPlayer";
    let unity_class = &env
        .find_class(unity_class_name)
        .expect("Failed to find class com/unity3d/player/UnityPlayer");

    let current_activity_obj: JObject = env
        .get_static_field(unity_class, "currentActivity", "Landroid/app/Activity;")
        .expect("Failed to get static field currentActivity")
        .l().unwrap();

    let ext_file_obj: JObject = env
        .call_method(
            current_activity_obj,
            "getExternalFilesDir",
            "(Ljava/lang/String;)Ljava/io/File;",
            &[JValueGen::from(&JObject::null())],
        )
        .expect("Failed to invoke getExternalFilesDir()")
        .l().unwrap();

    let file_string: JString = env
        .call_method(&ext_file_obj, "toString", "()Ljava/lang/String;", &[])
        .expect("Failed to invoke toString()")
        .l()
        .unwrap()
        .into();

    let str_data: String = env
        .get_string(&file_string)
        .expect("Failed to get string from jstring")
        .into();

    env.delete_local_ref(ext_file_obj).expect("Failed to delete local ref");
    env.delete_local_ref(file_string).expect("Failed to delete local ref");

    unsafe {
        DATA_DIR = Some(str_data.clone());
    }
}
