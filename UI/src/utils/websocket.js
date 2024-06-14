import WebSocketAsPromised from 'websocket-as-promised'
import Channel from 'chnl'
import { Notify } from 'quasar'
import { getToken } from './auth'
import store from '@/store' // 确保导入了 Vuex store
import router from '@/router' // 确保导入了 Vue Router

const wsUrl = process.env.VUE_APP_WS_URL
const wsOption = {
  packMessage: data => JSON.stringify(data),
  unpackMessage: data => JSON.parse(data),
  attachRequestId: (data, requestId) => Object.assign({ id: requestId, token: getToken() }, data),
  extractRequestId: data => data && data.id
}

const ws = new WebSocketAsPromised(wsUrl, wsOption)

ws.$eventEmitter = new Channel.EventEmitter()

function handleMessage(message) {
  console.log('消息到达:', message)

  if (message.status !== 200 && !message.ignoreError) {
    Notify.create({
      message: message.statusText,
      color: 'negative',
      icon: 'error'
    })

    if (message.status === 401) {
      router.push(`/login`)
    }
    return
  }
  if (message.eventName === 'Logout') {
    onLogout()
  }
  if (message.eventName) {
    ws.$eventEmitter.dispatch(message.eventName, message)
  }
}

async function onLogout() {
  console.log('websocket 登出')
  await store.dispatch('user/logout')
  router.push(`/login`)
}
ws.onUnpackedMessage.addListener(handleMessage)

// 重连逻辑
function connectWebSocket() {
  try {
    ws.open().then(() => {
      ws.sendRequest({
        name: 'Login',
        command: 'storeSession'
      })
      console.log('websocket 连接成功')
    })
  } catch (e) {
    console.error('websocket Error')
    console.error(e)
    // 如果连接失败，则尝试重新连接
    setTimeout(connectWebSocket, 3000) // 3秒后重新连接
  }
}

// 监听 WebSocket 的关闭事件，触发重连逻辑
ws.onClose.addListener((event) => {
  console.log('websocket 连接断开:', event)
  // 如果连接关闭了，尝试重新连接
  connectWebSocket()
})

// 初始连接
connectWebSocket()

export default ws
