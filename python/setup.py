import distutils.core
import Cython.Build
from distutils.core import setup
from distutils.extension import Extension
from Cython.Build import cythonize

distutils.core.setup(
    ext_modules=cythonize(
        Extension(
            "onebrc", ["onebrc.py","temprature_stat.py","file_chunk.py"],
            extra_compile_args=["-O3"],
            language="c++",
        ),
    ),
    )