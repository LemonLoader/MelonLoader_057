use std::fs;
use std::io::{Read, Write};
use std::ffi::CString;
use jni::JNIEnv;
use jni::objects::{JObject, JString, JValueGen, JObjectArray};
use libc::c_void;
use ndk_sys::{AAssetManager, AAssetManager_openDir};

use crate::melonenv::paths;

const COPY_DATA_STR: &str = "copyToData";

pub fn copy_melon(env: &mut JNIEnv) -> bool {
    let base = format!("{}/", paths::BASE_DIR.clone().display());

    unsafe {
        copy_file_or_dir("MelonLoader", &base, "", env);

        // TODO: implement these copy functions
        //copy_file_or_dir(COPY_DATA_STR, &base, "", env);
        //copy_file_or_dir("bin/Data/Managed/etc", &(base + "il2cpp/"), "etc", env);
        
        copy_file_or_dir("dotnet", format!("/data/data/{}", "com.SirCoolness.UnityTestModdingTarget").as_str(), "", env);
    }
    true
}

unsafe fn copy_file_or_dir(path: &str, base: &str, path_start: &str, env: &mut JNIEnv) {
    let asset_manager = get_android_asset_manager(env);
    let unity_class_name = "com/unity3d/player/UnityPlayer";
    let unity_class = &env
        .find_class(unity_class_name)
        .expect("Failed to find class com/unity3d/player/UnityPlayer");

    let current_activity_obj: JObject = env
        .get_static_field(unity_class, "currentActivity", "Landroid/app/Activity;")
        .expect("Failed to get static field currentActivity")
        .l().unwrap();

    let asset_manager_obj = env.call_method(current_activity_obj, "getAssets", "()Landroid/content/res/AssetManager;", &[]).unwrap().l().unwrap();

    // Convert path and base to jstring
    let path_string = env.new_string(path).unwrap();
    let base_string = env.new_string(base).unwrap();

    // Access the list method of AssetManager to get assets array
    //let asset_manager_class = env.get_object_class(asset_manager_obj);
    let assets_array: JObjectArray = env.call_method(&asset_manager_obj, "list", "(Ljava/lang/String;)[Ljava/lang/String;", &[JValueGen::from(&path_string)]).unwrap().l().unwrap().into();

    // Convert assets array length to Rust usize
    let assets_length = env.get_array_length(&assets_array).unwrap();
    let assets_size = assets_length as usize;

    if assets_size == 0 {
        copy_file(path, base, env);
    } else {
        let full_path = format!("{}/{}", base, path);

        // Create the directory if it doesn't exist
        create_directory(&full_path);

        // Iterate over assets and recursively call copy_file_or_dir
        for i in 0..assets_size {
            let asset = env.get_object_array_element(&assets_array, i as i32).unwrap();
            let jstr: JString = asset.into();
            let asset_str: String = env.get_string(&jstr).unwrap().into();
            let asset_path = format!("{}/{}", path, asset_str);
            env.delete_local_ref(jstr);

            copy_file_or_dir(&asset_path, base, &asset_path[path_start.len()..], env);
        }
    }

    // Release local references
    env.delete_local_ref(path_string);
    env.delete_local_ref(base_string);
}

unsafe fn copy_file(filename: &str, base: &str, env: &mut JNIEnv) {
    let asset_manager = get_android_asset_manager(env);
    let filename_cstr = CString::new(filename).unwrap();
    let asset = ndk_sys::AAssetManager_open(asset_manager, filename_cstr.as_ptr(), ndk_sys::AASSET_MODE_UNKNOWN as i32);

    // This is for copyToData. I would do this earlier, but it didn't work.
    if asset.is_null() {
        return;
    }

    // Not great, but it works.
    let mut out_filename = filename.to_string();
    if let Some(pos) = out_filename.find(COPY_DATA_STR) {
        out_filename.replace_range(pos..pos + COPY_DATA_STR.len(), "");
    }

    let full_path = format!("{}/{}", base, out_filename);
    if std::path::Path::new(&full_path).exists() {
        fs::remove_file(&full_path);
    }
    let mut output_stream = fs::File::create(full_path).unwrap();

    const BUFFER_SIZE: usize = 1024;
    let mut buffer = [0u8; BUFFER_SIZE];
    let mut total_bytes_read = 0;

    loop {
        let bytes_read = ndk_sys::AAsset_read(asset, buffer.as_mut_ptr() as *mut std::ffi::c_void, BUFFER_SIZE as u64);
        if bytes_read <= 0 {
            break;
        }

        output_stream.write_all(&buffer[0..bytes_read as usize]).unwrap();
        total_bytes_read += bytes_read as i32;
    }

    // Release the asset
    ndk_sys::AAsset_close(asset);

    output_stream.flush().unwrap();
}

fn create_directory(path: &str) -> bool {
    // Not great, but it works.
    let mut new_path = path.to_string();
    if let Some(pos) = new_path.find(COPY_DATA_STR) {
        new_path.replace_range(pos..pos + COPY_DATA_STR.len(), "");
    }

    if !fs::metadata(&new_path).is_ok() {
        if fs::create_dir(&new_path).is_err() {
            return false;
        }
    }

    true
}
fn get_android_asset_manager(env: &mut JNIEnv) -> *mut AAssetManager {
    let unity_class_name = "com/unity3d/player/UnityPlayer";
    let unity_class = &env
        .find_class(unity_class_name)
        .expect("Failed to find class com/unity3d/player/UnityPlayer");

    let current_activity_obj: JObject = env
        .get_static_field(unity_class, "currentActivity", "Landroid/app/Activity;")
        .expect("Failed to get static field currentActivity")
        .l().unwrap();

    let asset_manager = env.call_method(current_activity_obj, "getAssets", "()Landroid/content/res/AssetManager;", &[]);
    unsafe {
        return ndk_sys::AAssetManager_fromJava(env.get_native_interface(), asset_manager.unwrap().l().unwrap().as_raw());
    }
}