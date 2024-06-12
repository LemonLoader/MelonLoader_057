use std::{
    ffi::{c_char, c_void, CString},
    ptr::null_mut,
    sync::RwLock,
};
use libc::{dlopen, dlsym, RTLD_NOW};

use lazy_static::lazy_static;

use crate::{constants::DlopenFn, debug, errors::DynErr, hooks::NativeHook, internal_failure};

lazy_static! {
    pub static ref DLOPEN_HOOK: RwLock<NativeHook<DlopenFn>> =
        RwLock::new(NativeHook::new(null_mut(), null_mut()));
}

pub fn hook() -> Result<(), DynErr> {
    debug!("Attaching hook to dlopen")?;

    let libdl = CString::new("libdl.so").unwrap();
    let handle = unsafe { dlopen(libdl.as_ptr(), RTLD_NOW) };

    if handle.is_null() {
        debug!("Failed to load libdl.so");
        panic!();
    }

    let symbol_name = CString::new("dlopen").unwrap();
    let dlopen_ptr = unsafe { dlsym(handle, symbol_name.as_ptr()) };

    if dlopen_ptr.is_null() {
        debug!("Failed to get symbol dlopen");
        panic!();
    } else {
        println!("dlopen pointer: {:?}", dlopen_ptr);
    }

    let detour = detour as usize;

    let mut init_hook = DLOPEN_HOOK.try_write()?;
    *init_hook = NativeHook::new(dlopen_ptr, detour as *mut c_void);

    init_hook.hook()?;

    Ok(())
}

pub fn detour(lib: *const c_char, flags: i32) -> *mut c_void {
    detour_inner(lib, flags).unwrap_or_else(|e| {
        internal_failure!("dlopen detour failed: {e}");
    })
}

fn detour_inner(lib: *const c_char, flags: i32) -> Result<*mut c_void, DynErr> {
    let mut lib = lib;
    let mut casted_lib = unsafe { std::ffi::CStr::from_ptr(lib).to_str()? };
    debug!("Detouring dlopen: {casted_lib}")?;

    if (casted_lib.contains("MelonLoader/Dependencies/dotnet")) {
        lib = casted_lib.split('/').last().unwrap().as_ptr() as *const c_char;
        casted_lib = unsafe { std::ffi::CStr::from_ptr(lib).to_str()? };
        debug!("New detour path: {casted_lib}")?;
    }

    let trampoline = DLOPEN_HOOK.try_read()?;
    let result = trampoline(lib, flags);

    Ok(result)
}
