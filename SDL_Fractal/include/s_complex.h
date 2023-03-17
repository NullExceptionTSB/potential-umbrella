#pragma once
typedef struct _COMPLEX {
	double Re, Im;
}complex;

void ComplexSqr(complex* c);
void ComplexAdd(complex* dst, complex* src);