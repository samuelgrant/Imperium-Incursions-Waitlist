import React, { Component } from 'react';

const baseUri = "/api/v1/ui/"

export class Pilot extends Component {

    apiCall(id) {
        console.error(`(501) Open pilot window in game: ${id}`);
    }

    getId() {
        return this.props.pilot.id || -1;
    }

    getName() {
        return this.props.pilot.name || "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Corporation extends Component {

    apiCall(id) {
        console.error(`(501) Open corp window in game: ${id}`);
    }

    getId() {
        return this.props.corporation.id || -1;
    }

    getName() {
        return this.props.corporation.name || "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Alliance extends Component {

    apiCall(id) {
        console.error(`(501) Open alliance window in game: ${id}`);
    }

    getId() {
        return this.props.alliance.id || -1;
    }

    getName() {
        return this.props.alliance.name || "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Destination extends Component {

    apiCall(id) {
        console.error(`(501) Set destination to system: ${id}`);
    }

    getId() {
        return this.props.system.id || -1;
    }

    getName() {
        return this.props.system.name || "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Market extends Component {

    apiCall(id) {
        console.error(`(501) Open market item: ${id}`);
    }

    getId() {
        return this.props.item.id || -1;
    }

    getName() {
        return this.props.item.name || "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}