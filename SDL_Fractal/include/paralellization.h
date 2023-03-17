#pragma once
#include <SDL2/SDL.h>
typedef struct _WORKER_PARAM {
	SDL_Surface* surface;
	int ymin, ymax;
	unsigned int im;
	double remin, remax, immin, immax;
}wstat;

int Foreman(SDL_Window* wnd);
extern SDL_ThreadFunction Worker;