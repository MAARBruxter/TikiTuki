![image1](menuPixel)

![Unity](https://img.shields.io/badge/Engine-Unity%206-black?logo=unity)
![Status](https://img.shields.io/badge/Status-In%20Progress-yellow)
![License](https://img.shields.io/badge/License-MIT-green)
![CLA Required](https://img.shields.io/badge/CLA-Required-blue.svg)
![Itch.io](https://img.shields.io/badge/Play-Itch.io-red?logo=itch.io)

## Contexto / Propósito

Este videojuego es una práctica de clase del curso **GECEGS en Desarrollo de Videojuegos y Realidad Virtual** en el centro **CPIFP Alan Turing (Campanillas, Málaga)**.  
El objetivo principal es aplicar conceptos de desarrollo en Unity 2D, incluyendo mecánicas de plataformas, gestión de PowerUps, enemigos y recolección de objetos.

## Historia

**Hongui**, una pequeña seta curiosa, deja su hogar seguro para explorar el Bosque Brillante y recolectar diamantes mágicos.  
En su camino deberá esquivar águilas, ranas y caracoles mientras demuestra que, aunque seas pequeño, la valentía puede llevarte a grandes aventuras.

## Mecánicas

- **Sistema de daño**: Los enemigos causan daño al estilo clásico de plataformas 2D tipo Mario Bros.
- **Saltos**: Doble salto desde el suelo y salto único si se inicia desde el aire.
- **PowerUps**:
  - Vida extra
  - Invencibilidad temporal
- **Recolección de diamantes**: Recoger gemas incrementa la puntuación.
- **Enemigos**: Encontrarás en el camino águilas voladoras, ranas y caracoles.

## Código fuente

El proyecto está desarrollado en **Unity 6** con C#. Entre los scripts podemos ver:

- **AudioManager**: Controla gran parte de los sonidos, incluyendo interfaz, player, enemigos, gemas y efectos de PowerUps.  
- **GameManager**: Gestiona la lógica global del juego, estados de niveles y pausa.  
- **MenuPrincipal**: Controla los botones y paneles del menú principal.  
- **UIManager** *(desactivado)*: Gestionaría la activación de elementos gráficos en pantalla como HUD y mensajes.  
- **MobileControlsManager**: Detecta si el dispositivo es móvil y gestiona los controles táctiles.  
- **SimpleJoystick**: Joystick virtual 2D que detecta arrastre táctil para generar un vector de movimiento.  
- **Enemy**: Controla el movimiento, daño, efectos y sonidos de todos los enemigos.  
- **PlayerCollect**: Gestiona la recolección de gemas y notifica al GameManager.  
- **PlayerController**: Controla el movimiento del jugador, saltos y animaciones.  
- **PlayerHealth**: Gestiona la salud del jugador y efectos de PowerUps.  
- **Gem**: Controla el comportamiento de las gemas y efectos visuales al recolectarlas.  
- **PowerUp**: Gestiona los PowerUps (Vida e Invencibilidad) y sus efectos sobre el jugador.

Los scripts incluyen documentación interna y métodos de debug.

## Sistema de puntuación

- Cada diamante recolectado incrementa la puntuación del jugador. Al llegar a 10 diamantes avanzamos de nivel. 
- Los PowerUps no incrementan la puntuación directamente, pero ayudan a sobrevivir.  

## Créditos

**Desarrollo**  
Alberto Fernández Hidalgo  

**Tilesets**  
- Forest Lite Pixel Art Tileset  

**Personajes / Sprites**  
- Player Principal: Cute Mushroom Character Sprite  
- Enemigo Hombre-Rana: Isometric Frog Character  
- Enemigo Caracol: Snail Character 2.0  
- Gatos: CatPackFree  
- Águilas: 2DBirds Eagle  

**Animaciones**  
- Cute Mushroom Character Sprite  
- Isometric Frog Character  
- Snail Character 2.0  
- CatPackFree  
- 2DBirds Eagle  

**Efectos Visuales (VFX)**  
- msVFX Free Smoke Effects Pack  
- Lana Studio Hyper Casual FX  

**Sonidos**  
- Collectables Sound Pack  
- RPG Essentials Free (Leohpaz)  

## Juega ahora

Prueba el juego en **Itch.io**:  
[Super Hongobros en Itch.io](https://albertofernandezhid.itch.io/super-hongobros)
