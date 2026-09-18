# Actividad: patrones creacionales en dos clases

Esta actividad se dirige a estudiantes de quinto semestre que ya conocen programación orientada a objetos, interfaces y pruebas automatizadas. Trabajarás sobre una API que ya inicia, documenta sus endpoints y persiste datos. Tu responsabilidad se limita a los patrones creacionales pendientes.

## Resultado esperado

Al terminar, tu equipo entrega una implementación de Factory, Builder y Strategy + Factory que respeta las capas existentes. Los contratos públicos de ejemplo deben dejar de fallar sin modificar la arquitectura, las pruebas ni las entidades fuera de los archivos del ejercicio.

## Clase 1: crea órdenes sin acoplar las capas

Durante la primera clase, revisa la estructura `Api → Application → Domain ← Infrastructure`. Después completa Factory y Builder en los archivos identificados en el README.

Verifica que Application y Api no conozcan las clases concretas de órdenes. Conserva las reglas existentes de monto, cuentas, SWIFT, programación y estados.

## Clase 2: selecciona el cálculo de comisión

Durante la segunda clase, completa las estrategias de comisión y su Factory. Mantén la decisión de estrategia concentrada en la Factory.

Calcula las comisiones con estas reglas: nacional 1%, internacional 3% y programada 0.5%. Redondea cada resultado a dos decimales.

Ejecuta las pruebas. Revisa que los contratos públicos pasen y que las pruebas de arquitectura continúen protegiendo a Domain de dependencias externas.

## Usa IA como apoyo, no como sustituto

Puedes usar IA para pedir explicaciones conceptuales, interpretar mensajes de error y contrastar decisiones de diseño. Escribe tú la solución e intégrala en incrementos pequeños.

No solicites ni copies una implementación completa de los archivos pendientes. No modifiques, deshabilites ni simules pruebas.

## Entrega

Entrega el repositorio con la implementación de los archivos del ejercicio y una explicación breve, escrita por cada integrante, que responda:

- Por qué la Factory evita que otras capas conozcan órdenes concretas.
- Qué responsabilidad tiene el Builder antes de construir una orden.
- Por qué cada cálculo de comisión vive en una Strategy.
- Qué reglas de negocio permanecen en Domain.

## Calificación: doble nota

La actividad produce dos notas con el mismo peso.

| Nota | Peso | Evidencia |
|---|---:|---|
| Implementación del equipo | 50% | Los contratos públicos pasan, las pruebas de arquitectura permanecen intactas y la solución conserva los límites entre capas. |
| Sustentación individual | 50% | Cada estudiante explica sus decisiones, las validaciones necesarias y el papel de cada patrón en su propio código. |

La sustentación individual se realiza al final de la segunda clase. El docente puede pedir que un integrante describa cualquier parte de la solución entregada por su equipo.
