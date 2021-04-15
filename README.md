# Referral Plugin
> Allows server admins to reward players for referring other players as well as offer server referrals and promotional rewards via customizable codes

![](https://cdn.discordapp.com/attachments/377619690513498133/406183455123177481/OpenSauce.svg)
![](https://cdn.discordapp.com/attachments/330777295952543744/478325842188042241/license.svg)
![](https://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg) ![](https://forthebadge.com/images/badges/built-with-love.svg)
[![Patreon](https://img.shields.io/badge/patreon-donate-green.svg)](https://www.patreon.com/bePatron?u=847269)

This is a Torch plugin for community servers wishing to reward players for referrals and to offer promotions.

![](header.png)

## Installation

Torch Plugin

1. place the Referralplugin zip in the Plugins folder
2. get the guid from the manifest in the Referralplugin zip. (also can be found at top of readme)
3. In the Torch.cfg place the guid between the <plugins></plugins> tag.
4. launch torch, and the plugin can be found in the plugins tab of torch.

##Usage

Using the plugin
* Referral Rewards Enabled - Enables the Referral features .
* Promotion Rewards Enabled - Enables promotional reward features.
* Give Money - gives players money amount set as an award.
* Give Grid - gives grid set in UI as a reward to players.
* Server Referral Code - Code for server referrals
* Promotional Reward Code - Code for Promotion
* Server Referral Grid - Name of grid to be used as reward for server referrals
* Promotional Reward Grid - Name of grid to be used as reward for Promotions
* Player Referral Grid - Name of grid to be used as reward for player referral  
* Credit Amount - amount of credits to give as reward
* Admin uses !r save command when looking at the grid he wishes to use as a reward then puts grid name in the corresponding field in the UI.
* Player uses related command to get the reward. (see commands)
* Players can only use the referral code or the player referral one time so it can not be abused.
* Promotional codes can only but used one time per player but players can use as many unique codes as they wish.
  The code will only word if it is the current one set in the UI so players can not use old codes.
* The player who was used as the referrer in the player referral can claim a reward for referring there friend with the claim command.
* Referral Data tab hold all the information related to this plugin for any player who has used commands for this plugin.  
> Note - Grids for rewards should be saved with inventory that is wished to be given as a reward already loaded into it.
  this will spawn in the grid when the player claims the reward.

## Commands

* !r save - used to save a grid when looking at the grid
* !r testload <GridName> - Loads reward grid to ensure proper save
* !r player <player name or steam Id> - gives player referral reward
* !r code <Server Referral Code> - gives server referral reward
* !r promo <Promotional Code> - gives Promotional reward
* !r claim - gives player who was used as referrer referral reward 


## Release History

* 1.0
    * Initial Release
  
## Prerequisites

* Requires [Torch](https://torchapi.net/)

## Coming Features

* Nexus Support
* Database Support

> Note - Nexus Support is pending on a K,V  store feature being added to nexus

## License

This project is licensed under the AGPL-3.0 License - see the [LICENSE](LICENSE) file for details