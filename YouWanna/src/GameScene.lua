-- 游戏主场景
local GameScene = class("GameScene", cc.Scene)
local AIArt = require("src.AIArt")
local ObstacleSystem = require("src.ObstacleSystem")

function GameScene:ctor()
    print("创建游戏场景")
end

function GameScene:create()
    local scene = GameScene.new()
    scene:init()
    return scene
end

function GameScene:init()
    -- 初始化静态资源系统
    self.aiArt = AIArt
    self.aiArt:init()
    
    -- 创建加载界面
    self:createLoadingUI()
    
    -- 开始资源预加载
    self:startAssetGeneration()
    
    return true
end

function GameScene:createLoadingUI()
    -- 创建加载背景
    local loadingBg = cc.LayerColor:create(cc.c4b(0, 0, 0, 255))
    self:addChild(loadingBg)
    self.loadingBg = loadingBg
    
    -- 创建加载文字
    self.loadingLabel = cc.Label:createWithSystemFont("📦 正在加载游戏资源...", "Arial", 32)
    self.loadingLabel:setPosition(400, 350)
    self.loadingLabel:setColor(cc.c3b(255, 255, 255))
    self:addChild(self.loadingLabel)
    
    -- 创建进度文字
    self.progressLabel = cc.Label:createWithSystemFont("0%", "Arial", 24)
    self.progressLabel:setPosition(400, 300)
    self.progressLabel:setColor(cc.c3b(200, 200, 200))
    self:addChild(self.progressLabel)
    
    -- 创建提示文字
    local hintLabel = cc.Label:createWithSystemFont("正在加载静态游戏素材，请稍等...", "Arial", 18)
    hintLabel:setPosition(400, 250)
    hintLabel:setColor(cc.c3b(150, 150, 150))
    self:addChild(hintLabel)
    
    print("加载界面创建完成")
end

function GameScene:startAssetGeneration()
    -- 开始预加载资源
    self.aiArt:preGenerateAssets(function()
        print("🎉 资源加载完成，开始游戏!")
        self:onAssetsReady()
    end)
end

function GameScene:onAssetsReady()
    -- 移除加载界面
    if self.loadingBg then
        self.loadingBg:removeFromParent()
        self.loadingLabel:removeFromParent()
        self.progressLabel:removeFromParent()
    end
    
    -- 创建游戏内容
    self:createBackground()
    self:createPlayer()
    self:createObstacles()
    self:createUI()
    self:enableKeyboard()
    
    print("游戏准备完成!")
end

function GameScene:createBackground()
    -- 创建默认背景色
    local bg = cc.LayerColor:create(cc.c4b(135, 206, 235, 255)) -- 天蓝色
    self:addChild(bg)
    self.defaultBg = bg
    
    -- 尝试加载静态背景
    self.aiArt:createSprite("sky_background", function(bgSprite)
        if bgSprite then
            -- 设置背景精灵覆盖整个屏幕
            local winSize = cc.Director:getInstance():getWinSize()
            bgSprite:setPosition(winSize.width / 2, winSize.height / 2)
            
            -- 缩放背景以适应屏幕
            local spriteSize = bgSprite:getContentSize()
            local scaleX = winSize.width / spriteSize.width
            local scaleY = winSize.height / spriteSize.height
            bgSprite:setScale(math.max(scaleX, scaleY))
            
            self:addChild(bgSprite, -1) -- 添加到最底层
            
            -- 隐藏默认背景
            if self.defaultBg then
                self.defaultBg:setVisible(false)
            end
            
            print("✅ 静态背景加载完成")
        else
            print("🔄 使用默认背景色")
        end
    end)
    
    print("背景创建完成")
end

function GameScene:createPlayer()
    -- 使用静态玩家角色资源
    self.aiArt:createSprite("player_classic", function(sprite)
        if sprite then
            self.player = sprite
            -- 设置玩家初始位置
            self.player:setPosition(100, 100)
            self:addChild(self.player)
            print("✅ 静态玩家角色创建完成")
        else
            -- 备用方案：创建简单矩形
            self:createFallbackPlayer()
        end
    end)
    
    -- 玩家属性
    self.playerVelocity = cc.p(0, 0)
    self.playerSpeed = 200
    self.jumpPower = 400
    self.gravity = -800
    self.onGround = false
end

function GameScene:createFallbackPlayer()
    -- 备用玩家角色 (简单矩形)
    self.player = cc.DrawNode:create()
    local playerSize = cc.size(32, 32)
    local playerColor = cc.c4f(1, 0, 0, 1) -- 红色
    
    -- 绘制玩家矩形
    local vertices = {
        cc.p(0, 0),
        cc.p(playerSize.width, 0),
        cc.p(playerSize.width, playerSize.height),
        cc.p(0, playerSize.height)
    }
    self.player:drawPolygon(vertices, playerColor, 1, playerColor)
    
    -- 设置玩家初始位置
    self.player:setPosition(100, 100)
    self:addChild(self.player)
    
    print("🔄 备用玩家角色创建完成")
end

function GameScene:createObstacles()
    -- 初始化障碍物系统
    self.obstacleSystem = ObstacleSystem
    self.obstacleSystem:init(self, self.aiArt)
    
    -- 创建测试关卡
    self.obstacleSystem:createTestLevel()
    
    print("障碍物系统创建完成")
end

function GameScene:createUI()
    -- 创建死亡计数显示
    self.deathCount = 0
    self.deathLabel = cc.Label:createWithSystemFont("Deaths: 0", "Arial", 24)
    self.deathLabel:setPosition(100, 550)
    self.deathLabel:setColor(cc.c3b(255, 255, 255))
    self:addChild(self.deathLabel)
    
    -- 创建操作提示
    local hint = cc.Label:createWithSystemFont("WASD移动 空格跳跃", "Arial", 18)
    hint:setPosition(400, 50)
    hint:setColor(cc.c3b(255, 255, 255))
    self:addChild(hint)
    
    print("UI创建完成")
end

function GameScene:enableKeyboard()
    -- 启用键盘事件
    local listener = cc.EventListenerKeyboard:create()
    
    listener:registerScriptHandler(function(keyCode, event)
        self:onKeyPressed(keyCode, event)
    end, cc.Handler.EVENT_KEYBOARD_PRESSED)
    
    listener:registerScriptHandler(function(keyCode, event)
        self:onKeyReleased(keyCode, event)
    end, cc.Handler.EVENT_KEYBOARD_RELEASED)
    
    local eventDispatcher = self:getEventDispatcher()
    eventDispatcher:addEventListenerWithSceneGraphPriority(listener, self)
    
    print("键盘输入启用")
end

function GameScene:onKeyPressed(keyCode, event)
    if keyCode == cc.KeyCode.KEY_A or keyCode == cc.KeyCode.KEY_LEFT_ARROW then
        self.playerVelocity.x = -self.playerSpeed
    elseif keyCode == cc.KeyCode.KEY_D or keyCode == cc.KeyCode.KEY_RIGHT_ARROW then
        self.playerVelocity.x = self.playerSpeed
    elseif keyCode == cc.KeyCode.KEY_SPACE or keyCode == cc.KeyCode.KEY_W or keyCode == cc.KeyCode.KEY_UP_ARROW then
        -- I Wanna 特色：可以无限跳跃
        self.playerVelocity.y = self.jumpPower
    end
end

function GameScene:onKeyReleased(keyCode, event)
    if keyCode == cc.KeyCode.KEY_A or keyCode == cc.KeyCode.KEY_LEFT_ARROW or 
       keyCode == cc.KeyCode.KEY_D or keyCode == cc.KeyCode.KEY_RIGHT_ARROW then
        self.playerVelocity.x = 0
    end
end

function GameScene:onEnter()
    cc.Scene.onEnter(self)
    
    -- 启动游戏循环
    self:scheduleUpdateWithPriorityLua(function(dt)
        self:update(dt)
    end, 0)
end

function GameScene:update(dt)
    -- 应用重力
    self.playerVelocity.y = self.playerVelocity.y + self.gravity * dt
    
    -- 更新玩家位置
    local currentPos = cc.p(self.player:getPosition())
    local newPos = cc.p(
        currentPos.x + self.playerVelocity.x * dt,
        currentPos.y + self.playerVelocity.y * dt
    )
    
    -- 边界检查
    if newPos.x < 0 then newPos.x = 0 end
    if newPos.x > 768 then newPos.x = 768 end -- 800-32
    
    -- 地面碰撞检测 (简单实现)
    if newPos.y <= 50 then -- 地面高度
        newPos.y = 50
        self.playerVelocity.y = 0
        self.onGround = true
    else
        self.onGround = false
    end
    
    -- 应用新位置
    self.player:setPosition(newPos)
    
    -- 检查障碍物碰撞
    if self.obstacleSystem then
        local playerRect = cc.rect(newPos.x - 16, newPos.y - 16, 32, 32)
        local collision = self.obstacleSystem:checkCollision(playerRect)
        
        if collision == "death" then
            self:playerDie()
        end
    end
    
    -- 检查是否掉出屏幕 (死亡条件)
    if newPos.y < -100 then
        self:playerDie()
    end
end

function GameScene:playerDie()
    -- 玩家死亡
    self.deathCount = self.deathCount + 1
    self.deathLabel:setString("Deaths: " .. self.deathCount)
    
    -- 重置玩家位置
    self.player:setPosition(100, 100)
    self.playerVelocity = cc.p(0, 0)
    
    print("玩家死亡! 总死亡次数: " .. self.deathCount)
end

return GameScene
