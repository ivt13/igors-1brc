#cython: language_level=3

import cython

@cython.cclass
class FileChunk:
   
    file_path:cython.basestring = ""
    start_pos:cython.int
    length:cython.int




