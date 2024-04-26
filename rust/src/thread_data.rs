
#[derive(Clone)]
pub struct ThreadData {
    pub file_path: String,
    pub start_pos:u64,
    pub length:u64,
}

impl<'a> Default for ThreadData {
    fn default() -> ThreadData {
        ThreadData {
            file_path: String::new(),
            start_pos: 0,
            length: 0
        }
    }
}

