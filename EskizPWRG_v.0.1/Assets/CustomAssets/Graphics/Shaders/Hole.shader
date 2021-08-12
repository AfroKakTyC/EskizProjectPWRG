Shader "Holes/Depth Mask" {
	SubShader {
		// Очередь должа стоять после объектов которые смогут опускаться в дырку (шар,
		// дыра-цилиндр), но перед теми в которых мы хотим выколоть дыру (стол) 
		Tags { "Queue" = "Geometry-1" }
 
		// Не рисовать никаких цветов, только Z-буфер
 		ColorMask 0
		ZWrite On

		Stencil {
			Ref 2        // write a 1 into the buffer
			Comp always  // comparison for what is already there.
			Pass replace
		}
 
		// Во время прохода ничего не делаем 
		Pass {}
	}
}
