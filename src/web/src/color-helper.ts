export function getColor(seed: string) {
  let color = "#";
  for (let i = 0; i < 6; i++) {
    color += seed[Math.floor(Math.random() * 16)];
  }
  return color;
}
