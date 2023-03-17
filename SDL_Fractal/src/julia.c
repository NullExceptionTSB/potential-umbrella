#include <s_complex.h>
#include <constant.h>
#include <extern.h>
#include <options.h>
#include <interface.h>
#include <paralellization.h>

int JuliaWorker(wstat* stat) {
	uint32_t* fb = stat->surface->pixels;
	complex* c = malloc(sizeof(complex));
	complex* z = malloc(sizeof(complex));
	for (int x = 0; x < WND_WIDTH; x++) 
		for (int y = stat->ymin; y < stat->ymax; y++) {

			c->Re = MX;
			c->Im = MY;
			z->Re = ((double)x*((stat->remax - stat->remin))/WND_WIDTH) + stat->remin;
			z->Im = ((double)y*((stat->immax - stat->immin))/WND_HEIGHT) + stat->immin;

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