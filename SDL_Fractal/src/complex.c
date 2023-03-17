#include <s_complex.h>

void ComplexSqr(complex* c) {
	double re = c->Re,im = c->Im;
	c->Re = (re*re)-(im*im);
	c->Im = 2*re*im;
}

void ComplexAdd(complex* dst, complex* src) {
	dst->Re += src->Re;
	dst->Im += src->Im;
}