<script>
import { modifyGroup } from '@/api/group'
import { notifySuccess } from '@/components/iPrompt'

export default {
  data() {
    return {
      isShowModifyGroupDialog: false,
      initModifyGroupParams: {
        title: this.$t('modify'),
        tooltip: '',
        api: modifyGroup,
        // type 可接受的值：text/password/textarea/email/search/tel/file/number/url/time/date
        fields: [
          {
            name: '_id',
            type: 'text',
            label: this.$t('table.groupId'),
            required: false,
            readonly: false, // 为true时会被过滤
            hidden: true
          },
          {
            name: 'name',
            type: 'text',
            label: this.$t('table.subGroupName'),
            required: true
          },
          {
            name: 'description',
            type: 'textarea',
            label: this.$t('table.description'),
            required: false
          }
        ]
      }
    }
  },

  methods: {
    showModifyGroupDialog(data) {
      // 修改初始化参数
      if (data) {
        this.$set(
          this.initModifyGroupParams.fields[0],
          'default',
          data._id || ''
        )
        this.$set(
          this.initModifyGroupParams.fields[1],
          'default',
          data.name || ''
        )
        this.$set(
          this.initModifyGroupParams.fields[2],
          'default',
          data.description || ''
        )
      }

      this.initModifyGroupParams.title = this.newGroupTitle
      this.isShowModifyGroupDialog = true
      // 返回一个 Promise，以便在窗口关闭后执行回调
      return new Promise((resolve, reject) => {
        // 监听窗口关闭事件
        this.$once('refreshData', (result) => {
          if (result === 'success') {
            resolve() // 窗口成功关闭，执行 resolve 回调
          } else {
            reject() // 窗口关闭但未成功添加组，执行 reject 回调
          }
        })
      })
    },

    modifyGroup(data) {
      // 替换原来的数据
      const index = this.groupsOrigin.findIndex(g => g._id === data._id)
      if (index > -1) {
        this.groupsOrigin.splice(index, 1, data)
      }

      this.isShowModifyGroupDialog = false
      notifySuccess(this.$t('modifySuccess'))
      this.$emit('refreshData', 'success') // 通知父组件刷新数据，并传递成功标志
    }
  }
}
</script>
