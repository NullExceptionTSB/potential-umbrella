CC = gcc
LD = gcc
OUT = prog
CCFLAGS = 
CCLIBS = -lSDL2 -lm
all: $(OUT)

$(OUT): src/main.c
	gcc $(CCFLAGS) src/main.c $(CCLIBS) -o $(OUT)

.PHONY: clean
clean:
	rm -f $(OUT)
