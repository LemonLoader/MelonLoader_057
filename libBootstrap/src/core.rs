use ctor::ctor;

use crate::{errors::DynErr, internal_failure};

#[ctor]
fn startup() {
    init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e.to_string());
    });
}

fn init() -> Result<(), DynErr> {
    //console::init()?;
    #[cfg(not(target_os = "android"))]
    logger::init()?;

    //hooks::init_hook::hook()?;

    //console::null_handles()?;

    Ok(())
}

pub fn shutdown() {
    std::process::exit(0);
}