#include <SDL2/SDL.h>

#include <paralellization.h>
#include <extern.h>
#include <options.h>
#include <time.h>
#include <julia.h>
#include <mandelbrot.h>

SDL_ThreadFunction Worker = (SDL_ThreadFunction)DEFAULT_WORKER;

int Foreman(SDL_Window* wnd) {
	char t[48];
	wstat* states = malloc(sizeof(wstat)*THREADCOUNT);
	SDL_Surface* surface = SDL_GetWindowSurface(wnd);

	//double QRANGE = QR;
	int f=0;
	SDL_Thread** workers = malloc(sizeof(SDL_Thread*)*THREADCOUNT);
	
	clock_t s = clock();
	for (int i = 0; i < THREADCOUNT; i++){
		states[i].remax = X+QRANGE;
		states[i].remin = X-QRANGE;
		states[i].immax = Y+QRANGE;
		states[i].immin = Y-QRANGE;
		states[i].ymin = i*WND_H/THREADCOUNT;
		states[i].ymax = (i+1)*WND_W/THREADCOUNT;
		states[i].im = IM;
		states[i].surface = surface;	
	}
	
	for (;;) {
		SDL_FillRect(surface, NULL, COLS[0]);
		for (int i = 0; i < THREADCOUNT; i++) 
			workers[i] = SDL_CreateThread((SDL_ThreadFunction)Worker, "WORK", &states[i]);

		for (int i = 0; i < THREADCOUNT; i++) {
			SDL_WaitThread(workers[i], NULL);
			states[i].remax = X+QRANGE;
			states[i].remin = X-QRANGE;
			states[i].immax = Y+QRANGE;
			states[i].immin = Y-QRANGE;
			states[i].im = IM;
		}

		clock_t e = clock();
		sprintf(t, "%.2f FPS QRANGE=%.5f Frame %u I=%u", f/((double)((e-s))/(CLOCKS_PER_SEC)), QRANGE, f, IM);
		f++;
		SDL_SetWindowTitle(wnd, t);

		SDL_UpdateWindowSurface(wnd);	
	}
}