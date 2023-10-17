# Automatic-laser-cut-quotation
![ALCQ](https://github.com/piopiopio/Automatic-laser-cut-quotation/assets/14957816/9c98bf3b-d623-465c-adc5-f1accaf7a47b)

## Input
 * Dxf file containing the geometry to be cut, without descriptions,
 * Margin,
 * Number of pieces,
 * Material (e.g. Stainless Steel, Structural Steel, Aluminum),
 * Material thickness.

## Output: 
* Price.

 
## Algorithm, price calculation rule:
* Load geometry from dxf file.
* Parse geometry.
* Calculation of each length L<sub>i</sub> of the lines set according to the type, eg. straight line, curve etc.
* Calculation of the length of the line to cut L=∑<sub>0</sub><sup>i</sup>L<sub>i</sub> , L<sub>i</sub>-unit length.
* Calculation of the rectangle into which the uploaded detail, oriented according to the drawing, can be inserted. Parameters X_min; Y_min; X_max; Y_max determined according dxf file, field calculated as P=(X<sub>max</sub>-X<sub>min</sub>)(Y<sub>max</sub> -Y<sub>min</sub>)
* Material cost, proportional to the mass of a rectangle with a field P, with material thickness g multiplied by density ρ and the cost per kilogram of material C<sub>i</sub>, calculated as: Km=P*g*ρ*C<sub>i</sub>
* The cost of cutting determined as the product of the length of the cutting line L and the unit cost of cutting the material of the given thickness, C(g) is the value of the unit cost of cutting taken from the table of values (not functionally described because the cost changes in an irregular way due to changes in the thickness of the material to be cut consists of several dependencies, for thin sheets the important criterion is the speed of movement of the machine for thicker ones the power of the machine along with the thickness also changes the gas consumption) K<sub>c</sub>= L*C(g)
* Cost of preparation, a fixed fee independent of the quantity.
* The displayed price includes a margin m therefore the calculated price is given as: W= m*(K<sub>p</sub>+K<sub>c</sub>+K<sub>m</sub>)


