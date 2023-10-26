#include <time.h>
#include <stdlib.h>

#include <SDL2/SDL.h>

#define BC_OUTDIM 512

int xstate;
typedef struct _BC_ZONE {
    double a[4][4];
}bicube_zone;

bicube_zone* zone_create(int** pts, int channel) {
    bicube_zone* r = malloc(sizeof(bicube_zone));
    for (int i = 0; i < 4; i++) 
        for (int j = 0; j < 4; j++) {
            r->a[i][j] = (pts[j][i] & (0xFF << (8*channel))) >> (8*channel);
            r->a[i][j] /= 255.0;
        }
}

double cubic_interp(double a0, double a1, double a2, double a3, double x) {
    double A = (a3 - a2) - (a0 - a1);
    double B = (a0 - a1) - A;
    double C = a2 - a0;
    double D = a1;

    double res = A*x*x*x + B*x*x + C*x + D;
    
    if (res > 1.0) res = 1.0;
    else if (res < 0.0) res = 0.0;
    return res;
}

double bicubic_interp(double x, double y, bicube_zone* zone) {
    double xs[4];
    for (int i = 0; i < 4; i++)
        xs[i] = cubic_interp(zone->a[i][0], zone->a[i][1], zone->a[i][2], zone->a[i][3], x);
    return cubic_interp(xs[0], xs[1], xs[2], xs[3], y);
}

int xorshift() {
    xstate ^= xstate << 7;
    xstate ^= xstate >> 13;
    xstate ^= xstate << 11;
    return xstate * 0x2545F4914F6CDD1DULL;
}

void rerender(SDL_Window* wnd) {
    SDL_Color matrix[5][5];
    for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            *(int*)(&(matrix[x][y])) = xorshift() & 0x00FFFFFF;
    
    int** zonetemp = malloc(sizeof(int*)*4);
    for (int i = 0; i < 4; i++)
        zonetemp[i] = malloc(sizeof(int)*4);

    bicube_zone* zones[4][4][3];

    for (int i = 0; i < 5; i++)
        for (int j = 0; j < 5; j++) {
            
            for (int mx = 0; mx < 4; mx++)
                for (int my = 0; my < 4; my++) {
                    int sx = mx + (i-1);
                    int sy = my + (j-1);

                    if (sx < 0) sx = 0;
                    if (sy < 0) sy = 0;

                    if (sx > 3) sx = 3;
                    if (sy > 3) sy = 3;

                    zonetemp[mx][my] = *((int*)(&(matrix[sx][sy])));
                }

            zones[i][j][0] = zone_create(zonetemp, 0);
            zones[i][j][1] = zone_create(zonetemp, 1);
            zones[i][j][2] = zone_create(zonetemp, 2);
        }

    SDL_Surface* surf = SDL_GetWindowSurface(wnd);

    SDL_FillRect(surf, NULL, SDL_MapRGB(surf->format, 0, 0, 0));
    int* fb = surf->pixels;

    for (int x = 0; x < BC_OUTDIM; x++) 
        for (int y = 0; y < BC_OUTDIM; y++) {
            int zonex = x/(BC_OUTDIM/4);
            int zoney = y/(BC_OUTDIM/4);

            double  xf = x,
                    yf = y;
            xf /= (BC_OUTDIM/4);
            yf /= (BC_OUTDIM/4);

            xf = SDL_fmod(xf, 1);
            yf = SDL_fmod(yf, 1);

            double  rf = bicubic_interp(xf, yf, zones[zonex][zoney][2]), 
                    gf = bicubic_interp(xf, yf, zones[zonex][zoney][1]), 
                    bf = bicubic_interp(xf, yf, zones[zonex][zoney][0]);

            fb[x+y*BC_OUTDIM] = SDL_MapRGB(surf->format, rf*255, gf*255, bf*255);
        }

    SDL_UpdateWindowSurface(wnd);
}

int main(int argc, char* argv[]) {
    xstate = time(NULL);
    SDL_Init(SDL_INIT_VIDEO);

    SDL_Window* wnd = SDL_CreateWindow("bcinterp", 
        SDL_WINDOWPOS_UNDEFINED,
        SDL_WINDOWPOS_UNDEFINED,
        512,
        512, 
        0
    );
    rerender(wnd);
    for(;;){
        SDL_Event e;
        while (SDL_PollEvent(&e)) {
            switch (e.type){
                case SDL_QUIT:
                    exit(0);
                    break;
                case SDL_KEYDOWN:
                    switch (e.key.keysym.scancode) {
                        case SDL_SCANCODE_R:
                            rerender(wnd);
                            break;
                    }
                    break;
            }
        }
    }


}