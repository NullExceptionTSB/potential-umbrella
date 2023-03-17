CC = gcc
ARGS = -O3 -flto -march=native
LIBS = -lSDL2
OUT = out

all: $(OUT)

$(OUT): src/*.c
	gcc $(ARGS) src/*.c $(LIBS) -o $(OUT)
.PHONY: clean
clean:
	rm -f $(OUT)
