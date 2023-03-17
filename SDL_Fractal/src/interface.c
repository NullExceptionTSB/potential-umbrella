#include <SDL2/SDL.h>
#include <extern.h>
#include <paralellization.h>
#include <mandelbrot.h>
#include <julia.h>

int J = 0;
void handle_key(SDL_Scancode code) {
    switch(code) {
        case SDL_SCANCODE_LEFT:
            X-=QRANGE/8;
            break;
        case SDL_SCANCODE_RIGHT:
            X+=QRANGE/8;
            break;
        case SDL_SCANCODE_UP:
            Y-=QRANGE/8;
            break;
        case SDL_SCANCODE_DOWN:
            Y+=QRANGE/8;
            break;

        case SDL_SCANCODE_W:
            QRANGE /= 2;
            break;
        case SDL_SCANCODE_S:
            QRANGE *= 2;
            break;

        case SDL_SCANCODE_Q:
            IM++;
            break;
        case SDL_SCANCODE_E:
            IM--;
            break;

        case SDL_SCANCODE_C:
            if (J) {
                J = 0;
                Worker = (SDL_ThreadFunction)MandelbrotWorker;
                X = MX;
                Y = MY;
            } else {
                J = 1;
                Worker = (SDL_ThreadFunction)JuliaWorker;
                MX = X;
                MY = Y;
            }
            break;

        case SDL_SCANCODE_J:
            MX-=MI;
            break;
        case SDL_SCANCODE_L:
            MX+=MI;
            break;
        case SDL_SCANCODE_I:
            MY-=MI;
            break;
        case SDL_SCANCODE_K:
            MY+=MI;
            break;
        
        case SDL_SCANCODE_U:
            MI*=10;
            break;
        case SDL_SCANCODE_O:
            MI/=10;
            break;

        default: break;
        
    }        
}

