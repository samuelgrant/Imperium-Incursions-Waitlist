import React, { Component } from 'react';

const baseUri = "/api/v1/ui/"

export class Pilot extends Component {

    apiCall(id) {
        $.ajax({
            type: 'post',
            url: `/api/esi-ui/show-info`,
            data: { target_id: id }
        }).done((fleets) => {
            this.setState({ fleets: fleets });
        }).fail((err) => {
            console.error(`React/EsiUi {Pilot@apiCall} - Error requesting ESI UI Showinfo`, err.responseText);
        })  
    }

    getId() {
        if (this.props.pilot)
            return this.props.pilot.characterID;

        return 0;
    }

    getName() {
        if (this.props.pilot)
            return this.props.pilot.characterName;

        return "";
    }
    target_id

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Corporation extends Component {

    apiCall(id) {
        $.ajax({
            type: 'post',
            url: `/api/esi-ui/show-info`,
            data: { target_id: id }
        }).done((fleets) => {
            this.setState({ fleets: fleets });
        }).fail((err) => {
            console.error(`React/EsiUi {Corporation@apiCall} - Error requesting ESI UI Showinfo`, err.responseText);
        })  
    }

    getId() {
        if (this.props.corporation)
            return this.props.corporation.id;
        
        return -1;
    }

    getName() {
        if(this.props.corporation)
            return this.props.corporation.name;

        return "";
    }

    render() {
        return (<a onClick={this.apiCall.bind(this, this.getId())}>{this.getName()}</a>)
    }
}

export class Alliance extends Component {

    apiCall(id) {
        $.ajax({
            type: 'post',
            url: `/api/esi-ui/show-info`,
            data: { target_id: id }
        }).done((fleets) => {
            this.setState({ fleets: fleets });
        }).fail((err) => {
            console.error(`React/EsiUi {Alliance@apiCall} - Error requesting ESI UI Showinfo`, err.responseText);
        })  
    }

    getId() {
        if (this.props.alliance)
            return this.props.alliance.id;

        return -1;
    }

    getName() {
        if (this.props.alliance)
            return this.props.alliance.name;

        return "";
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