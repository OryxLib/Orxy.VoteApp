//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    pickarray: ["文本", "图片"],
    pickIndex: '0',
    uploadBucket: '',
    previewImgSrc: '',
    msgData: '',
    motto: 'Hello World',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    bgImg: 'https://mioto.milbit.com/WxqAppBg.jpg'
  },
  //事件处理函数
  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function () {
    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }
  },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },
  bindPickerChange: function (e) {
    this.setData({
      pickIndex: e.detail.value
    })
  },
  textInput: function (e) {
    this.setData({
      msgData: e.detail.value
    })
  },
  textConfirm: function (e) {
    this.setData({
      msgData: e.detail.value
    })
  },
  takePhoto: function () {
    var _this = this;
    wx.chooseImage({
      count: 1, // 默认9
      sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
      sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
      success: function (res) {
        // 返回选定照片的本地文件路径列表，tempFilePath可以作为img标签的src属性显示图片
        var tempFilePaths = res.tempFilePaths
        _this.setData({
          previewImgSrc: tempFilePaths[0]
        })
        wx.showModal({
          title: '确认',
          content: '是否发送图片?',
          success: function (res) {
            if (res.confirm) {
              //console.log('用户点击确定')
              _this.sendImg();
            } else if (res.cancel) {
              _this.setData({
                previewImgSrc: ''
              })
            }
          }
        })
      }
    })
  },
  postMsg: function (e) {

    var _this = this;
    if (_this.data.msgData || _this.data.previewImgSrc) {
      wx.showModal({
        title: '提示',
        content: '确认发送消息吗?',
        success: function (res) {
          if (res.confirm) {
            if (_this.data.msgData) {
              _this.sendTxt()
            }
            // if (_this.data.previewImgSrc) {
            //   _this.sendImg()
            // }
          } else if (res.cancel) {
            
          }
        }, fail: function (err) {
          console.log(err)
        }
      })
    }
    // switch (this.data.pickIndex) {
    //   case '0':
    //     this.sendTxt()
    //     break;
    //   case '1':
    //     this.sendImg()
    //     break;
    // }

  },
  sendTxt: function () {
    var _msgData = {
      nickName: app.globalData.userInfo.nickName,
      avarta: app.globalData.userInfo.avatarUrl,
      msg: this.data.msgData,
      msgType: 'txt'
    };
    console.log('sending txt')
    this.sendData(JSON.stringify(_msgData))
    this.setData({
      msgData: ''
    })
  },
  sendImg: function () {
    var _this = this;
    wx.request({
      url: app.globalData.baseUrl + '/game/GetToken?key=Linengneng',
      success: function (res) {
        console.log(res.data)
        _this.uploadImg(_this.data.previewImgSrc, res.data.token)
      },
      error: function () { }
    })
  },
  uploadImg: function (tmpPath, token) {
    var _this = this;
    console.log('upload img')
    wx.uploadFile({
      url: 'https://upload-z1.qiniup.com' || 'https://up-z1.qiniu.com', //仅为示例，非真实的接口地址
      filePath: tmpPath,
      name: 'file',
      formData: {
        'token': token,
        'key': (new Date() - 0).toString() + app.globalData.userInfo.nickName + '.jpg'
      },
      success: function (res) {
        var data = res.data
        console.log(res)
        var resObj = JSON.parse(res.data);
        var key = resObj.key;
        var imgUrl = 'https://mioto.milbit.com/' + key;

        var msgData = {
          nickName: app.globalData.userInfo.nickName,
          avarta: app.globalData.userInfo.avatarUrl,
          msg: imgUrl,
          msgType: 'img',
          date: new Date()
        };
        _this.sendData(JSON.stringify(msgData))
        _this.setData({
          previewImgSrc: ''
        })
      }, error: function (err) {
        console.log(err)
      },
      complete: function (res) {
        console.log(res)
      }
    })
  },
  sendData: function (sendingData) {
    console.log('sending request')
    wx.request({
      url: app.globalData.baseUrl + '/WxqFunc/PostCheckingData?key=Linengneng',
      method: 'post',
      data: {
        msg: sendingData
      },
      success: function (res) {
        console.log(res)
        wx.showToast({
          title: '发送成功',
        })
      },
      error: function () {
        wx.showToast({
          title: '发送失败',
        })
      },
      complete: function (res) {
        console.log(res)
      }
    })
  },
  // gobackVote: function () {
  //   wx.redirectTo({
  //     url: '/pages/index/index',
  //   })
  // }
})
