import pako from 'pako'

export const UngzipToString = (data) => {
  return new Promise((resolve, reject) => {
    try {
      if (data.constructor === Blob) {
        const reader = new FileReader()
        reader.readAsArrayBuffer(data)
        reader.onload = (result) => {
          const unzipped = pako.ungzip(result, { to: 'string' })
          resolve(unzipped)
        }
      } else if (data.constructor === ArrayBuffer) {
        const unzipped = pako.ungzip(data, { to: 'string' })
        resolve(unzipped)
      } else reject('解压缩失败: 不是Blob或ArrayBuffer数据')
    } catch (err) {
      reject(err)
    }
  })
}

export const GZipToByteArray = (data) => {
  return new Promise((resolve, reject) => {
    try {
      let zipped
      if (data.constructor === Blob) {
        const reader = new FileReader()
        reader.readAsArrayBuffer(data)
        reader.onload = (result) => {
          zipped = pako.gzip(result)
          resolve(zipped)
        }
      } else if (data.constructor === ArrayBuffer) {
        zipped = pako.gzip(data)
        resolve(zipped)
      } else if (data.constructor === String) {
        zipped = pako.gzip(data)
        resolve(zipped)
      } else {
        zipped = pako.gzip(JSON.stringify(data))
      }
      if (zipped) {
        resolve(zipped)
      } else reject('不支持的可压缩格式')
    } catch (err) {
      reject(err)
    }
  })
}

/**
 * CheckSum
 * @param {Blob | ArrayBuffer} data
 * @returns
 */
export const CheckSum = async (data) => {
  try {
    let bytes = []
    if (data.constructor === Blob) {
      bytes = await new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = () => resolve(new Uint8Array(reader.result))
        reader.onerror = (error) => reject(error)
        reader.readAsArrayBuffer(data)
      })
    } else if (data.constructor === ArrayBuffer) {
      bytes = new Uint8Array(data)
    }
    let sum = 0
    bytes.forEach((b) => (sum ^= b))
    return sum
  } catch (err) {
    return 0
  }
}
