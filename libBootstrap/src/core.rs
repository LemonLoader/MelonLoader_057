use ctor::ctor;

use crate::{errors::DynErr, internal_failure, logging::logger, console, hooks, base_assembly};

#[cfg_attr(
    not(target_os = "android"),
    ctor
)]
#[no_mangle]
fn startup() {
    init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e.to_string());
    });
}

fn init() -> Result<(), DynErr> {
    console::init()?;

    #[cfg(not(target_os = "android"))]
    logger::init()?;

    hooks::init_hook::hook()?;

    console::null_handles()?;

    base_assembly::init(crate::runtime!()?)?;

    Ok(())
}

pub fn shutdown() {
    std::process::exit(0);
}