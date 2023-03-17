#include <stdio.h>
#include <unistd.h>
#include <time.h>

#include <SDL2/SDL.h>

#include <s_complex.h>
#include <constant.h>
#include <options.h>
#include <extern.h>
#include <paralellization.h>

int MandelbrotWorker(wstat* stat) {
	uint32_t* fb = stat->surface->pixels;
	complex* c = malloc(sizeof(complex));
	complex* z = malloc(sizeof(complex));
	for (int x = 0; x < WND_WIDTH; x++) 
		for (int y = stat->ymin; y < stat->ymax; y++) {

			c->Re = ((double)x*((stat->remax - stat->remin))/WND_WIDTH) + stat->remin;
			c->Im = ((double)y*((stat->immax - stat->immin))/WND_HEIGHT) + stat->immin;
			z->Re = c->Re;
			z->Im = c->Im;

			for (unsigned int i = 0; i < stat->im; i++) {
				ComplexSqr(z);
				ComplexAdd(z, c);

				if (z->Re > SQRT2 || z->Im > SQRT2) {
					fb[x+y*WND_WIDTH] = COLS[i%COL_COUNT];
					break;
				}
			}
	}

	free(c);
	free(z);

	return 0;
}