/**
 * 数字转为16进制字符串，失败时返回null
 * @param {number} num
 * @param {boolean} withPrefix
 * @returns
 */
export function numberToHexString(num, withPrefix) {
  try {
    let hexStr = num.toString(16) // 转换为16进制字符串
    let length = hexStr.length
    length = Math.floor(length / 2) * 2 + (length % 2) * 2
    if (length > 4) length = Math.floor(length / 4) * 4 + ((length % 4) / 2) * 4
    let fixLength = length - hexStr.length
    if (fixLength > 0) for (var i = 0; i < fixLength; i++) hexStr = '0' + hexStr
    return withPrefix ? '0x' + hexStr : hexStr
  } catch {
    return null
  }
}

/**
 * 16进制字符串转为数字，失败时返回null
 * @param {string} hexStr
 * @returns
 */
export function hexStringToNumber(hexStr) {
  try {
    return parseInt(hexStr, 16)
  } catch {
    return null
  }
}

/**
 * 分页
 */
export class Paging {
  processing = false
  pageIndex = 1
  pageSize = 10
  pagingSelections = []
  constructor(size) {
    this.pageSize = !size ? 10 : size
  }
  start = () => {
    this.processing = true
  }
  stop = () => {
    this.processing = false
  }
  reset = () => {
    this.pageIndex = 1
    this.pagingSelections = []
  }
  sizeChange = (val) => {
    this.pageSize = val
  }
  indexChange = (val) => {
    this.pageIndex = val
  }
  setSelection = (val) => {
    this.pagingSelections = val
  }
  clearSelection = () => {
    this.pagingSelections = []
  }
}

export class Filter {
  filterKey = ''
  selections = []
  constructor() {}
  clear = (fullClean) => {
    this.filterKey = ''
    if (fullClean) {
      this.selections.forEach((r) => {
        if (r.checked === undefined) return
        r.checked = true
      })
    }
  }
  copy = (filter) => {
    this.filterKey = filter.filterKey
    this.selections.forEach((r) => {
      if (r.checked === undefined) return
      var f = filter.selections.find((r2) => !!r2.label && r2.label === r.label)
      if (f) r.checked = f.checked
    })
  }
}

/**
 * 根据class获取子节点中的第一个匹配的DomObject
 * @param
 * @returns
 */
export function findDomObjectByClassName(element, className) {
  try {
    if (` ${element.className} `.indexOf(` ${className} `) > -1) return element
    else if (element.childNodes && element.childNodes.length > 0) {
      for (var index in element.childNodes) {
        var result = findDomObjectByClassName(
          element.childNodes[index],
          className
        )
        if (result) return result
      }
    }
  } catch {
    return null
  }
}

export function writeClipboard(value) {
  if (
    !!navigator.clipboard &&
    typeof navigator.clipboard.write === 'function'
  ) {
    if (typeof value === 'string' && !!value.trim()) {
      navigator.clipboard.writeText(value).then(
        () => {
          window.$message.success('复制成功')
        },
        (err) => {
          window.$message.error(`复制失败: ${err}`)
        }
      )
    } else if (value) {
      navigator.clipboard.write(value).then(
        () => {
          window.$message.success('复制成功')
        },
        (err) => {
          window.$message.error(`复制失败: ${err}`)
        }
      )
    }
  }
}
