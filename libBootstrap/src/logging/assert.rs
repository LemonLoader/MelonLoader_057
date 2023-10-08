/// Throws an internal failure with the given message
///
/// This logs your message to file, creates a message box, and then panics.
/// It uses the same syntax as _format!_
///
/// # Examples
///
/// ```
/// # use utils::assert;
/// assert::internal_failure!("This is an internal failure");
/// ```
#[cfg(not(target_os = "android"))] 
#[macro_export]
macro_rules! internal_failure {
    ($($arg:tt)*) => {{
        let msg = &format_args!($($arg)*).to_string();

        std::println!("{}", msg);
        let _ = $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Error, msg);
        let _ = msgbox::create("Internal Failure", msg, msgbox::IconType::Error);
        std::process::exit(-1);
    }};
}

#[cfg(target_os = "android")] 
#[macro_export]
macro_rules! internal_failure {
    ($($arg:tt)*) => {{
        let msg = &format_args!($($arg)*).to_string();
        let mut prefix: String = "INTERNAL FAILURE: ".to_string();
        prefix.push_str(msg);
        let _ = $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Error, prefix.as_str());
        panic!(); // std::process::exit(-1) does not kill the app on Android, panic!() does
    }};
}
