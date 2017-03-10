attribute vec3 vertexPosition;

// varying vec2 fPosition;

void main(void) {
  gl_Position = vec4(vertexPosition,  1.0);
}
