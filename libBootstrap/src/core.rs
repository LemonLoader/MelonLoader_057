use ctor::ctor;

#[ctor]
fn startup() {
    #[cfg(target_os = "android")]
    android_log::init("MelonLoader").unwrap();

    info!("Initializing bootstrap...");
}

pub fn shutdown() {
    info!("Shutting down...");
    std::process::exit(0);
}
