const { MessageFactory } = require('botbuilder');

/**
 * QnA Maker から返ってきた答えを適切な形にしてクライアントへ返す
 * 
 * @param {string} qnaResult QnA Maker から返ってきたanswer
 * @returns Activity
 */
module.exports.createAnswer = function(context, qnaResult) {

    if (qnaResult.context && qnaResult.context.prompts && qnaResult.context.prompts.length > 0) {
        // answerにfollow-up promptが付いている
        return createPrompt(qnaResult);
    } else {
        return qnaResult.answer;
    }
}

/**
 * follow-up prompt をクライアントへ返す
 * 
 * @param {QnAMakerResult} qnaResult 
 * @returns Activity
 */
function createPrompt(qnaResult) {
    let title = qnaResult.answer;
    let prompts = qnaResult.context.prompts;
    let buttons = [];

    // DisplayOrderで並べ替え
    prompts.sort(function(a, b) {
        if (a.displayOrder == b.displayOrder) {
            return 0;
        } else if (a.displayOrder < b.displayOrder) {
            return -1;
        }
        return 1;
    });

    // follow-up promptのボタンを作る
    for (let prompt of prompts) {
        buttons.push(prompt.displayText);
    }

    return MessageFactory.suggestedActions(buttons, title);
}// JavaScript source code
