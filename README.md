# img3 - Gray-Scott Reaction-Diffusion Simulation

## Runtime controls
* `a`/`A` - Decrease/increase chemical A diffusion rate.
* `b`/`B` - Decrease/increase chemical B diffusion rate.
* `f`/`F` - Decrease/increase feed rate.
* `k`/`K` - Decrease/increase kill rate.
* `l` - Toggle fluid flow effects.  Default is on.
* `t`/`T` - Decrease/increase time scale.
* `w` - Toggle random walk through the feed/kill parameter space.  Default is on.
* `1` - 3x3 diffusion laplacian; 1/r^2 weights.  This is the default.
* `2` - 3x3 diffusion laplacian; 1/r weights.
* `3` - 3x3 cross (no corners) diffusion laplacian; 1/r weights.
* `4` - 3x3 cross (no corners) diffusion laplacian; 1/r weights; add a small amount of noise.
* `<space>` - Wipe the display.
* `.` - Put a dot of chemical B in the center.
* `<mouse click>` - Put a dot of chemical B under the mouse cursor.
* `<mouse click and drag>` - Create a trail of chemical B.
* `Esc` - If maximized, restore window; else quit.

## References
* [Gray-Scott tutorial](https://www.karlsims.com/rd.html)
* [Palettes - 1999](https://iquilezles.org/www/articles/palettes/palettes.htm)
