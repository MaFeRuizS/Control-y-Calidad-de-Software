# GUÍA DE CONFIGURACIÓN - JUEGO EDUCAE
## Configuración paso a paso en Unity

---

## PARTE 1: IMPORTAR SPRITES Y SCRIPTS

### 1.1 Importar Sprites
1. Descarga la carpeta "Sprites" que te voy a compartir
2. En Unity, ve a la ventana **Project** (abajo)
3. Arrastra toda la carpeta **Sprites** a tu carpeta **Assets/Sprites**
4. Unity importará automáticamente las imágenes

### 1.2 Importar Scripts
1. Descarga la carpeta "Scripts" que te voy a compartir
2. Arrastra todos los archivos .cs a tu carpeta **Assets/Scripts**
3. Espera a que Unity compile (verás una barra de progreso abajo)

---

## PARTE 2: CONFIGURAR LA ESCENA PRINCIPAL

### 2.1 Crear la Escena
1. En **Project**, ve a **Assets/Scenes**
2. Clic derecho → **Create → Scene**
3. Nómbrala: **"MainGame"**
4. Doble clic para abrirla

### 2.2 Configurar la Cámara
1. Selecciona **Main Camera** en la ventana **Hierarchy** (izquierda)
2. En **Inspector** (derecha):
   - Position: X=0, Y=0, Z=-10
   - Size: 5
   - Background: Color gris claro (R:236, G:240, B:241)

### 2.3 Crear el GameManager
1. Clic derecho en **Hierarchy** → **Create Empty**
2. Nómbralo: **"GameManager"**
3. Con GameManager seleccionado, en **Inspector**:
   - Clic en **Add Component**
   - Busca y agrega el script **"GameManager"**

### 2.4 Crear el Background
1. Clic derecho en **Hierarchy** → **2D Object → Sprite**
2. Nómbralo: **"Background"**
3. En **Inspector**:
   - Arrastra el sprite **GameBackground** al campo **Sprite**
   - Ajusta la escala para cubrir toda la pantalla

---

## PARTE 3: CREAR EL ELEMENTO QUE CAE (PREFAB)

### 3.1 Crear el Prefab de Tarjeta
1. Clic derecho en **Hierarchy** → **2D Object → Sprite**
2. Nómbralo: **"MathCard"**
3. En **Inspector**:
   - Sprite: Arrastra **Card_Template** desde Sprites/Matematicas
   - Add Component → **Box Collider 2D**
   - Add Component → Script **"MathElement"**

### 3.2 Agregar Texto a la Tarjeta
1. Clic derecho en **MathCard** → **UI → Text - TextMeshPro**
2. Si aparece una ventana de TMP, acepta importar
3. Nómbralo: **"OperationText"**
4. En **Inspector**:
   - Text: "2 + 2"
   - Font Size: 36
   - Alignment: Centrado
   - Color: Blanco

### 3.3 Conectar referencias en MathElement
1. Selecciona **MathCard**
2. En el componente **Math Element**:
   - Arrastra **OperationText** al campo **Operation Text**
   - Arrastra el **Sprite Renderer** al campo **Card Sprite**

### 3.4 Crear el Prefab
1. Arrastra **MathCard** desde Hierarchy a **Assets/Prefabs**
2. Ahora puedes eliminar MathCard de la Hierarchy

---

## PARTE 4: CREAR LOS BOTONES DE CATEGORÍAS

### 4.1 Crear Canvas para UI
1. Clic derecho en **Hierarchy** → **UI → Canvas**
2. En Canvas, ajusta:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler → UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

### 4.2 Crear Botón de Categoría (ejemplo: número 4)
1. Clic derecho en **Canvas** → **UI → Button - TextMeshPro**
2. Nómbralo: **"Button_4"**
3. Posiciónalo en la parte inferior de la pantalla
4. En **Inspector**:
   - Width: 120, Height: 120
   - Add Component → Script **"CategoryButton"**
   - En CategoryButton, establece **Category Value: 4**

### 4.3 Personalizar el botón
1. Expande **Button_4** en Hierarchy
2. Selecciona el hijo **"Text (TMP)"**
3. Cambia el texto a: **"4"**
4. Font Size: 48
5. Color: Blanco

### 4.4 Repetir para otros números
Crea botones para: **5, 6, 7, 8, 9, 10, 12, 15, 16, 20, 25**
Distribúyelos en la parte inferior de la pantalla

---

## PARTE 5: CREAR EL HUD (INTERFAZ)

### 5.1 Panel de Puntuación
1. Clic derecho en **Canvas** → **UI → Panel**
2. Nómbralo: **"ScorePanel"**
3. Posición: Esquina superior izquierda
4. Dentro de ScorePanel, crea:
   - **UI → Text - TextMeshPro**: "Puntos: 0"
   - Nómbralo: **"ScoreText"**

### 5.2 Panel de Tiempo
1. Similar al ScorePanel
2. Posición: Esquina superior derecha
3. Texto: **"TimeText"** → "02:00"

### 5.3 Panel de Nivel
1. Similar a los anteriores
2. Posición: Centro superior
3. Texto: **"LevelText"** → "Nivel: 1"

### 5.4 Barra de Vidas (Corazones)
1. Clic derecho en **Canvas** → **UI → Image**
2. Nómbralo: **"Heart_1"**
3. Sprite: Arrastra **Heart_Full** desde Sprites/UI
4. Duplica 2 veces más para tener 3 corazones
5. Posiciónalos en fila en la esquina superior

---

## PARTE 6: CONECTAR TODO EN GAMEMANAGER

### 6.1 Seleccionar GameManager
En Hierarchy, selecciona **GameManager**

### 6.2 Conectar referencias en Inspector
En el componente **Game Manager**, arrastra:
- **Score Text** → ScoreText
- **Time Text** → TimeText  
- **Level Text** → LevelText
- **Life Hearts** → Array de 3 elementos (Heart_1, Heart_2, Heart_3)
- **Element Prefab** → El prefab MathCard que creaste
- **Spawn Point** → Crea un Empty GameObject arriba de la pantalla
- **Ground Limit** → Crea un Empty GameObject abajo de la pantalla
- **Category Buttons** → Array con todos los botones de categoría

---

## PARTE 7: CREAR PUNTOS DE REFERENCIA

### 7.1 Spawn Point
1. Clic derecho en **Hierarchy** → **Create Empty**
2. Nómbralo: **"SpawnPoint"**
3. Posición: X=0, Y=5, Z=0

### 7.2 Ground Limit
1. Clic derecho en **Hierarchy** → **Create Empty**
2. Nómbralo: **"GroundLimit"**
3. Posición: X=0, Y=-5, Z=0

### 7.3 Conectar al GameManager
Selecciona **GameManager** y arrastra estos objetos a sus campos correspondientes

---

## PARTE 8: CREAR PANELES DE GAME OVER Y LEVEL COMPLETE

### 8.1 Panel Game Over
1. Clic derecho en **Canvas** → **UI → Panel**
2. Nómbralo: **"GameOverPanel"**
3. Hazlo del tamaño de toda la pantalla
4. Color: Negro semi-transparente (A: 200)
5. Dentro, crea:
   - Text: "GAME OVER"
   - Button: "Reintentar" (conecta al método RestartGame del GameManager)
6. **Desactiva el panel** (checkbox en Inspector)

### 8.2 Panel Level Complete
1. Similar a Game Over
2. Texto: "¡NIVEL COMPLETADO!"
3. Button: "Siguiente Nivel" (conecta al método NextLevel)
4. **Desactiva el panel**

### 8.3 Conectar paneles al GameManager
Arrastra ambos paneles a los campos correspondientes en GameManager

---

## PARTE 9: CONFIGURAR INPUT

### 9.1 Crear InputManager
1. Clic derecho en **Hierarchy** → **Create Empty**
2. Nómbralo: **"InputManager"**
3. Add Component → Script **"InputManager"**

### 9.2 Configurar teclas
En InputManager, configura:
- Category Keys: [1, 2, 3, 4]
- Category Values: [4, 5, 6, 10]

---

## PARTE 10: PROBAR EL JUEGO

### 10.1 Guardar la escena
- File → Save (Ctrl+S)

### 10.2 Probar
- Presiona el botón **Play** ▶️ en la parte superior
- Deberías ver:
  - Una tarjeta cayendo con una operación
  - Botones de categorías funcionando
  - Sistema de vidas
  - Puntuación
  - Timer

### 10.3 Controles
- **Teclas 1, 2, 3, 4**: Seleccionar categorías
- **Clic en botones**: Clasificar elemento
- **R**: Reiniciar
- **ESC**: Salir

---

## PARTE 11: AJUSTES FINALES

### 11.1 Build Settings
1. File → Build Settings
2. Add Open Scenes
3. Platform: PC, Mac & Linux Standalone
4. Target Platform: Windows
5. Architecture: x86_64

### 11.2 Player Settings
- Company Name: Tu nombre/equipo
- Product Name: EDUCAE
- Icon: (opcional) Crea un ícono simple

---

## RESOLUCIÓN DE PROBLEMAS COMUNES

### Error: "TextMeshPro not found"
- Window → TextMeshPro → Import TMP Essential Resources

### Los sprites no se ven
- Selecciona los sprites en Project
- En Inspector, cambia Texture Type a "Sprite (2D and UI)"

### Los botones no funcionan
- Asegúrate de que el Canvas tenga un EventSystem
- Hierarchy → Clic derecho → UI → Event System

### El elemento no cae
- Verifica que SpawnPoint esté arriba (Y positivo)
- Verifica que GroundLimit esté abajo (Y negativo)
- Revisa que elementFallSpeed sea > 0 en GameManager

---

## ¡LISTO!

Ahora tienes un juego funcional con:
✅ Generación automática de operaciones matemáticas
✅ Sistema de clasificación
✅ Sistema de vidas
✅ Puntuación
✅ Timer
✅ Niveles progresivos
✅ Controles por teclado

**Próximos pasos:**
- Agregar más disciplinas (Ciencias, Español)
- Integrar ESP32
- Añadir efectos de sonido
- Mejorar animaciones
