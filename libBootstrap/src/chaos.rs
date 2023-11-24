use std::os::unix::io::FromRawFd;
use std::os::unix::io::IntoRawFd;
use std::process;
use std::io::{self, BufRead, BufReader};
use std::ffi::CString;
use android_liblog_sys::__android_log_write;

use crate::error;
use crate::log;

pub unsafe fn main() {
    std::env::set_var("COREHOST_TRACE", "1");
    std::env::set_var("COREHOST_TRACE_VERBOSITY", "3");

    let mut pipes: [libc::c_int; 2] = [0; 2];
    libc::pipe(pipes.as_mut_ptr());
    libc::dup2(pipes[1], libc::STDERR_FILENO);
    let input_file = libc::fdopen(pipes[0], CString::new("r").unwrap().as_ptr());
    let mut read_buffer = [0; 512];
    loop {
        libc::fgets(read_buffer.as_mut_ptr() as *mut libc::c_char, read_buffer.len() as i32, input_file);

        __android_log_write(4, CString::new("MelonLoader").unwrap().as_ptr(), read_buffer.as_ptr() as *const libc::c_char);
    }
}