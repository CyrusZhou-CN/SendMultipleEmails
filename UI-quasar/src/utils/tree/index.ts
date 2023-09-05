/**
 * 遍历数据树
 * @param trees 多个树
 * @param fn
 */
export function walkTree(trees: object[], fn, options = null) {
  if (!trees || trees.length < 1) return
  if (typeof fn !== 'function') return

  const fullOptions = Object.assign({ children: 'children' }, options)
  const { children } = fullOptions

  trees.forEach((tree) => {
    fn(tree)

    if (tree[children]) {
      walkTree(tree[children], fn, options)
    }
  })
}
