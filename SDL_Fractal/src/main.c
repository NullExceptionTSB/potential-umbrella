#include <SDL2/SDL.h>

#include <extern.h>
#include <options.h>
#include <paralellization.h>
#include <interface.h>

double QRANGE = 2;
unsigned int IM = 10;

double X = 0;
double Y = 0;

double MX = 0;
double MY = 0;

double MI = 0.0001;

int WND_H;
int WND_W;

uint32_t COLS[COL_COUNT];

//X0 = 0
uint8_t lerp(int x, int x1, uint8_t y0, uint8_t y1) {
	return y0 + (x)*(y1-y0)/(x1);
}

int main() {
	SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS);

#ifndef WND_FULLSCREEN
	WND_W = WND_WIDTH;
	WND_H = WND_HEIGHT;
	SDL_Window* wnd = SDL_CreateWindow(WND_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, WND_WIDTH, WND_HEIGHT, 0);
#else
	SDL_Window* wnd = SDL_CreateWindow(WND_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, WND_W, WND_H, 0);
	SDL_SetWindowFullscreen(wnd, SDL_WINDOW_FULLSCREEN_DESKTOP);

	WND_W = 1366;
	WND_H = 768;
#endif
	SDL_Surface* s = SDL_GetWindowSurface(wnd);
	for (int i = 0; i < COL_COUNT/2; i++) 
		COLS[COL_COUNT-i] = COLS[i] = SDL_MapRGBA(
			s->format, 
			lerp(i, COL_COUNT/2, BG_R, FG_R), 
			lerp(i, COL_COUNT/2, BG_G, FG_G), 
			lerp(i, COL_COUNT/2, BG_B, FG_B), 
			255);

	SDL_CreateThread((SDL_ThreadFunction)Foreman, "DRAW", wnd);

	for (;;) {
		SDL_Event event;
		while (SDL_PollEvent(&event)) {
			switch (event.type) {
				case SDL_QUIT:
					exit(0);
				case SDL_KEYDOWN:
					handle_key(event.key.keysym.scancode);
					break;
			}
		}
	}
}
