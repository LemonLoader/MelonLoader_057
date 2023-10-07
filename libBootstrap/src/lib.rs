#[macro_use]
extern crate log;

pub mod core;
#[cfg(target_os = "android")]
pub mod core_android;