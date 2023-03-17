#include <stdio.h>
#include <unistd.h>
#include <time.h>

#include <SDL2/SDL.h>

#define PI			(99733.0/31746.0)
#define E			(106.0/39.0)

#define THREADCOUNT	8

#define WND_TITLE 	"SDL_01"
#define WND_WIDTH 	512
#define WND_HEIGHT	512

#define FPS_EVERY	30

#define TI			0.002
#define TMIN		-100
#define TMAX		100

#define MT			0.001


typedef struct _WORKER_PARAM {
	SDL_Surface* surface;
	double tmin, tmax, m;
	uint32_t c_b, c_f;
}wstat;

int Worker(wstat* stat) {
	uint32_t* fb = stat->surface->pixels;
	for (double t = stat->tmin; t < stat->tmax; t+=TI) {
		double a = SDL_pow(E, SDL_cos(t*stat->m)) - 2*SDL_cos(4*t) - SDL_pow(SDL_sin(t/12),5);

		int x = (WND_WIDTH/10*SDL_sin(t)*a + WND_WIDTH/2),
			y = (WND_HEIGHT/10*-SDL_cos(t)*a + WND_HEIGHT/2);

		fb[x+y*WND_WIDTH] = stat->c_f;
	}
}

int Foreman(SDL_Window* wnd) {
	char t[24];
	wstat* states = malloc(sizeof(wstat)*THREADCOUNT);
	SDL_Surface* surface = SDL_GetWindowSurface(wnd);
	uint32_t c_b = SDL_MapRGBA(surface->format, 0, 0, 255, 255),
			c_f = SDL_MapRGBA(surface->format, 255, 255, 0, 255);

	double tdiff = (TMAX - TMIN)/THREADCOUNT;
	double m = 1;
	int f = 0;

	SDL_Thread** workers = malloc(sizeof(SDL_Thread*)*THREADCOUNT);
	clock_t s = clock();
	for (int i = 0; i < THREADCOUNT; i++){
		states[i].c_b = c_b;
		states[i].c_f = c_f;
		states[i].tmin = TMIN+tdiff*i;
		states[i].tmax = TMIN+tdiff*(i+1);
		states[i].m = m;
		states[i].surface = surface;	
	}
	
	for (;;) {
		SDL_FillRect(surface, NULL, c_b);
		for (int i = 0; i < THREADCOUNT; i++) 
			workers[i] = SDL_CreateThread((SDL_ThreadFunction)Worker, "WORK", &states[i]);

		
		for (int i = 0; i < THREADCOUNT; i++) {
			SDL_WaitThread(workers[i], NULL);
			states[i].m = m;
		}
		clock_t e = clock();
		sprintf(t, "%.2f FPS m=%.5f Frame %u", f/((double)((e-s))/(CLOCKS_PER_SEC)), m, f++);
		SDL_SetWindowTitle(wnd, t);

		SDL_UpdateWindowSurface(wnd);	
		m += MT;
	}
}

int Draw(SDL_Window* wnd) {
	char t[24];
	SDL_Surface* surface = SDL_GetWindowSurface(wnd);
	unsigned int* fb = surface->pixels;

	int f = 0;
	double m = 0.00;
	clock_t s = clock();

	uint32_t c_b = SDL_MapRGBA(surface->format, 0, 0, 255, 255),
			 c_f = SDL_MapRGBA(surface->format, 255, 255, 0, 255);

	for (;;) {
		SDL_FillRect(surface, NULL, c_b);
		for (double t = TMIN; t < TMAX; t += TI) { 
			double a = SDL_pow(E, SDL_cos(t*m)) - 2*SDL_cos(4*t) - SDL_pow(SDL_sin(t/12),5);

			int x = (WND_WIDTH/10*SDL_sin(t)*a + WND_WIDTH/2),
				y = (WND_HEIGHT/10*-SDL_cos(t)*a + WND_HEIGHT/2);

			fb[x+y*WND_WIDTH] = c_f;
		}

		m+=MT;

		if (f++ == FPS_EVERY) {
			clock_t e = clock();
			f = 0;
			sprintf(t, "%.2f FPS m=%.5f", (double)(FPS_EVERY*CLOCKS_PER_SEC/(e-s)), m);
			SDL_SetWindowTitle(wnd, t);
			s = e;
		}
		
		SDL_UpdateWindowSurface(wnd);	
	}
}

int main() {
	SDL_Init(SDL_INIT_VIDEO);

	SDL_Window* wnd = SDL_CreateWindow(WND_TITLE, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, WND_WIDTH, WND_HEIGHT, 0);
	SDL_Thread* drawthread = SDL_CreateThread((SDL_ThreadFunction)Foreman, "DRAW", wnd);

	for (;;) {
		
		SDL_Event event;
		while (SDL_PollEvent(&event)) {
			switch (event.type) {
				case SDL_QUIT:
					exit(0);
				
			}
		}
	}
}
