/* might be used later, idk

use std::ffi::CString;
use std::os::unix::ffi::OsStrExt;
use std::os::unix::prelude::PermissionsExt;
use std::path::Path;
use std::fs;
use std::ptr;
use std::error::Error;
use std::io;
use std::os::raw::{c_char, c_void};
use libc::{c_int, off_t};
use libc::{O_RDONLY, O_CREAT, O_WRONLY};
use libc::{lseek, sendfile};
use std::mem;
use std::ptr::null_mut;
use std::cmp;
use libc::{RTLD_NOW, RTLD_GLOBAL};
use std::os::unix::io::AsRawFd;

use crate::debug;
use crate::errors::DynErr;
use crate::log;

pub struct AssemblyLoader {
    assembly_temp_path: String,
}

impl AssemblyLoader {
    pub fn initialize() -> Result<Self, Box<dyn Error>> {
        let mod_temp_path = format!("/data/data/{}", "com.SirCoolness.UnityTestModdingTarget");
        debug!("AssemblyLoader Temp: {}", &mod_temp_path);

        // Create the AssemblyLoader struct with the temp path
        let assembly_loader = AssemblyLoader {
            assembly_temp_path: mod_temp_path.clone(),
        };

        // Clear the temp directory
        assembly_loader.clear_temp_directory()?;

        Ok(assembly_loader)
    }

    fn clear_temp_directory(&self) -> Result<(), Box<dyn Error>> {
        let dir = fs::read_dir(&self.assembly_temp_path)?;

        for entry in dir {
            let entry = entry?;
            let path = entry.path();

            if path.is_file() && path.extension().unwrap_or_default() == "so" {
                if let Err(err) = fs::remove_file(&path) {
                    log!(
                        "Failed to delete: {} error: {}",
                        path.display(),
                        err,
                    );
                } else {
                    log!("Deleted: {}", path.display());
                }
            }
        }

        Ok(())
    }

    pub unsafe fn open(&self, library_path: &str) -> Result<*mut c_void, DynErr> {
        let full_path = CString::new(library_path)?;

        if let Some(filename) = Path::new(library_path).file_name().and_then(|s| s.to_str()) {
            let infile = std::fs::File::open(library_path)?;
            let filesize = lseek(infile.as_raw_fd(), 0, libc::SEEK_END);
            lseek(infile.as_raw_fd(), 0, libc::SEEK_SET);

            let temp_path = format!("{}/{}", self.assembly_temp_path, filename);
            let outfile = std::fs::File::create(temp_path.clone())?;
            outfile.set_permissions(std::fs::Permissions::from_mode(0777));

            sendfile(
                outfile.as_raw_fd(),
                infile.as_raw_fd(),
                null_mut(),
                filesize as usize,
            );

            let mode_bits = libc::S_IRUSR | libc::S_IWUSR | libc::S_IXUSR | libc::S_IRGRP | libc::S_IWGRP | libc::S_IXGRP;
            let permissions = std::fs::Permissions::from_mode(mode_bits);
            std::fs::set_permissions(temp_path.as_str(), permissions)?;

            Ok(unsafe { libc::dlopen(CString::new(temp_path)?.as_ptr(), RTLD_NOW | RTLD_GLOBAL) })
        } else {
            Ok(unsafe { libc::dlopen(full_path.as_ptr(), RTLD_NOW | RTLD_GLOBAL) })
        }
    }
}*/

use std::fs;
use std::io;
use std::path::{Path, PathBuf};

pub fn main() -> io::Result<()> {
    let source_dir = Path::new("/sdcard/Android/data/com.SirCoolness.UnityTestModdingTarget/files/MelonLoader/dotnet/");
    let dest_dir = Path::new("/data/data/com.SirCoolness.UnityTestModdingTarget/dotnet/");

    // Check if the source directory exists.
    if !source_dir.exists() {
        eprintln!("Source directory does not exist.");
        return Ok(());
    }

    // Create the destination directory if it doesn't exist.
    if !dest_dir.exists() {
        fs::create_dir_all(&dest_dir)?;
    }

    // Recursively copy the source directory to the destination directory.
    copy_dir_recursive(&source_dir, &dest_dir)?;

    println!("Directory copied successfully.");
    Ok(())
}

fn copy_dir_recursive(source: &Path, destination: &Path) -> io::Result<()> {
    for entry in fs::read_dir(source)? {
        let entry = entry?;
        let source_path = entry.path();
        let destination_path = destination.join(entry.file_name());

        if source_path.is_dir() {
            // If it's a directory, recursively copy it.
            fs::create_dir_all(&destination_path)?;
            copy_dir_recursive(&source_path, &destination_path)?;
        } else {
            // If it's a file, copy it.
            fs::copy(&source_path, &destination_path)?;
        }
    }

    Ok(())
}