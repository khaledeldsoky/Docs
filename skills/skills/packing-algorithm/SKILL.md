---
name: packing-algorithm
description: Use for questions about the bin packing algorithms (MaxRects, Guillotine, Metaheuristic), scoring functions in maxrects.py, geometry (Rect), LeftoverAnalysis, and optimizer.py routing.
---

# Packing Algorithms

## Geometry (`geometry.py`)
```python
@dataclass
class Rect:
    x: float; y: float; width: float; height: float
    # Properties: right (x+width), top (y+height), area (w*h)
    # Methods: fits(w,h), contains(other), overlaps(other)
```

## Algorithm Selection (`optimizer.py`)
Routes based on `algorithm` field in `OptimizeRequest`:
- `"maxrects"` → `optimize_maxrects()` (default)
- `"guillotine"` → `optimize_guillotine()`
- `"metaheuristic"` → `optimize_maxrects_metaheuristic()`

## MaxRects (`maxrects.py`)

### Insert / Scoring Loop
`insert(w, h, remaining_dims)` in `MaxRectsBin`:
1. Iterates free rects × both orientations
2. Calls `_score_rect(free, pw, ph, min_rem_dim)` for BSSF candidate score
3. Applies modifiers: orientation uniformity, sister bonus, gravity, global frag, aspect alignment, grid alignment
4. Selects best (lowest score)
5. **Look-ahead** for primary pieces (>15% sheet area): simulates next 3 pieces for both orientations, picks orientation with higher aggregate density
6. Places piece, splits free rects, prunes

### Scoring: `_score_rect()`
```python
fragment_penalty - largest_reward + waste + sq_penalty + edge + future_waste
```
- `fragment_penalty`: `len(new_rects) * 100000` — penalizes fragmentation
- `largest_reward`: `areas[0]²` — squared, dominates all other terms; preserves one big block
- `waste`: `free.area - (pw*ph)` — raw dead space
- `sq_penalty`: `sum(_squareness(r)) / 10` — penalizes slivers
- `edge`: `_edge_score()` — rewards corner/edge leftovers, penalizes internal holes; ignores fragments below 2% sheet area
- `future_waste`: `+50000` per gap narrower/shorter than `min_rem_dim`

### Scoring Modifiers (applied in `insert()` after `_score_rect`)

**Orientation Uniformity** (`±50000`): bias toward orientation used by existing identical pieces.
```python
if sister_rot is not None:
    if rotated == sister_rot: score -= 50000
    else: score += 20000
```

**Sister Reward** (`−30000`): flush adjacency against identical piece with same orientation.

**Gravity Bonus** (better than sheet-edge hugging):
- Sheet edge: −15000 (1 edge), −30000 (2+ edges)
- Piece flush: −25000 each (max 3 = −75000)
- Boundary gap penalty: +40000 if edge-touching piece dimension < `min_rem_dim`

**Global Fragmentation Cost**: `(1 − maxArea/totalFreeArea) × 300000` — penalizes scattering waste.

**Aspect Ratio Alignment** (`+50000`): cross-weaving penalty for high-AR (≥2.5) pieces placed perpendicular to adjacent high-AR pieces.

**Grid Alignment Penalty** (`+40000`): identical piece with different orientation in same row/column.

### Guillotine (`GuillotineBin`)
- Longer-axis split rule
- Same `_score_rect` scoring as MaxRects
- Called via `optimize_guillotine()` which reuses `_pack_maxrects()` order

### Metaheuristic
`optimize_maxrects_metaheuristic()` — 100 iterations:
1. Shuffles piece sequence
2. Flips large pieces (>10% sheet area) randomly
3. Each iteration: sorts by `(-area, width, height)`, packs via `_pack_maxrects()`
4. Picks best result: first by fewest sheets, then by highest used area

### Sorting
`_pack_maxrects()` uses `sorted(pieces, key=lambda p: (-p[0]*p[1], p[0], p[1]))` where pieces are `(width, height, quantity)` tuples expanded to individual placements.

### LeftoverAnalysis
`analyze_leftovers(sheet_width, sheet_height, placements)` produces:
- `total_waste_area`, `num_leftover_regions`, `largest_leftover`
- `reusable_leftovers` / `unusable_leftovers`
- `avg_aspect_ratio`, `rectangularity_score`, `corner/edge/internal_leftovers`
- Threshold: large fragments (≥1% sheet area) classified; corner=edge_count≥2, edge=edge_count≥1, internal=otherwise
