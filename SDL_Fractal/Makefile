#NOTE: for some reason optimization levels other than -O0 cause crashes
#	   and **lower** performafce. The reason for this is unknown.
CC = gcc
ARGS = -Wall -Wextra -Wpedantic -Werror -flto -O0 -Iinclude
LIBS = -lSDL2

OUT = sdlf

all: $(OUT)

$(OUT): build build/main.o src/main.c build/mandelbrot.o build/julia.o build/complex.o build/interface.o build/paralellization.o
	$(CC) $(ARGS) build/main.o build/julia.o build/mandelbrot.o build/paralellization.o build/interface.o build/complex.o $(LIBS) -o $(OUT)

build/main.o: src/main.c include/options.h
	$(CC) -c $(ARGS) src/main.c -o build/main.o

build/complex.o: src/complex.c
	$(CC) -c $(ARGS) src/complex.c -o build/complex.o

build/interface.o: src/interface.c
	$(CC) -c $(ARGS) src/interface.c -o build/interface.o

build/paralellization.o: src/paralellization.c
	$(CC) -c $(ARGS) src/paralellization.c -o build/paralellization.o

build/julia.o: src/julia.c
	$(CC) -c $(ARGS) src/julia.c -o build/julia.o

build/mandelbrot.o: src/mandelbrot.c
	$(CC) -c $(ARGS) src/mandelbrot.c -o build/mandelbrot.o

build:
	@-mkdir build

.PHONY: clean
clean:
	rm -rf build
	rm -f $(OUT)
